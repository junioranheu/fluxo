// Definir os elementos nas variáveis;
var $txtNomeCompleto = $('#txtNomeCompleto');
var $txtEmail = $('#txtEmail');
var $txtUsuario = $('#txtUsuario');
var $txtSenha = $('#txtSenha');
var $btnCriarConta = $('#btnCriarConta');

// Como o usuário não pode estar logado nessa página,
// Existe um bug que quando o usuário usa o botão de voltar página,
// a váriavel idUsuarioLogado não atualiza, só quando dá o lindo F5...
// Portanto, se o usuário entrar na tela usando o botão de voltar: Dê um refresh (F5) na página;
window.addEventListener("pageshow", function (event) {
    // https://stackoverflow.com/questions/43043113/how-to-force-reloading-a-page-when-using-browser-back-button;
    var historyTraversal = event.persisted ||
        typeof window.performance !== "undefined" && window.performance.navigation.type === 2;

    if (historyTraversal) {
        window.location.reload();
    }
});

// Outros;
$(document).ready(function () {
    // Focar no input;
    $txtNomeCompleto.focus();

    // Permitir inserir apenas letras e espaços no nome completo;
    $txtNomeCompleto.on('keypress', function (e) {
        var regex = new RegExp("^[a-zA-Z ]*$");

        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });

    // Permitir inserir apenas letras e números no nome de usuário;
    $txtUsuario.on('keypress', function (e) {
        var regex = new RegExp("^[a-zA-Z0-9]*$");

        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });

    // Não permitir utilizar espaço no e-mail;
    $txtEmail.keypress(function (e) {
        if (e.which === 32) {
            return false;
        }
    });

    // Se o usuário tiver nos input do criar, simule o click no botão;
    $('#txtNomeCompleto, #txtEmail, #txtUsuario, #txtSenha').keypress(function (event) {
        if (event.which === 13 || event.keyCode === 13) {
            $btnCriarConta.click();
        }
    });
});

// Criar conta e seus processos (validações e etc);
$(document).ready(function () {
    $btnCriarConta.click(function () {
        // Pegar os valores dos campos;
        var nomeCompleto = $txtNomeCompleto.val();
        var email = $txtEmail.val().toLowerCase();
        var nomeUsuario = $txtUsuario.val();
        var senha = $txtSenha.val();

        // Verificar se os campos estão preenchidos;
        // Verificação do nome #1: nome preenchido?;
        if (!nomeCompleto) {
            Aviso('warning', 'Parece que você esqueceu de colocar o seu nome ' + Emoji('Aviso'), true, 4000);
            $txtNomeCompleto.focus();
            return;
        }

        // Verificação do nome #2: pelo menos 03 caracteres?;
        if (nomeCompleto.length < 3) {
            Aviso('warning', 'Seu nome não pode ter menos de 03 caracteres ' + Emoji('Aviso'), true, 4000);
            $txtNomeCompleto.focus();
            return;
        }

        // Verificação do nome #3: se existe pelo menos um espaço (dois nomes), false = não;
        var reg = new RegExp("(\\w+)(\\s+)(\\w+)");
        if (reg.test(nomeCompleto) === false) {
            Aviso('warning', nomeCompleto + ' é um belo nome, mas cadê seu sobrenome? ' + Emoji('Aviso'), true, 4000);
            $txtNomeCompleto.focus();
            return;
        }

        // Verificação de e-mail #1: e-mail preenchido?;
        if (!email) {
            Aviso('warning', 'Parece que você esqueceu de colocar o seu e-mail ' + Emoji('Aviso'), true, 4000);
            $txtEmail.focus();
            return;
        }

        // Verificação de e-mail #2: e-mail válido?;
        if (ChecarEmail(email) === false) {
            Aviso('error', 'Parece que esse e-mail não é válido... ' + Emoji('Aviso'), true, 4000);
            $txtEmail.focus();
            return;
        }

        // Verificação de nome de usuário #1: nome de usuário preenchido?;
        if (!nomeUsuario) {
            Aviso('warning', 'Parece que você esqueceu de colocar um nome de usuário (apelido que será utilizado no sistema) ' + Emoji('Aviso'), true, 4000);
            $txtUsuario.focus();
            return;
        }

        // Verificação de nome de usuário #2: pelo menos 03 caracteres?;
        if (nomeUsuario.length > 20 || nomeUsuario.length < 4) {
            Aviso('warning', 'O nome de usuário não pode ter não pode ter menos de 4 e nem mais de 10 caracteres, e agora está com ' + nomeUsuario.length + '! ' + Emoji('Aviso'), true, 7000);
            $txtUsuario.focus();
            return;
        }

        // Verificação de senha #1: senha preenchida?;
        if (!senha) {
            Aviso('warning', 'Parece que você esqueceu de colocar sua senha ' + Emoji('Aviso'), true, 4000);
            $txtSenha.focus();
            return;
        }

        // Verificação da senha #2 (fica no site.js): realizar uma série de verificações, se alguma retornar falso, aborte;
        if (ChecarSenha(senha) === false) {
            return;
        }

        // console.log('Todos os parâmetros necessários foram devidamente preenchidos');

        // Atribuir o nome formatado para a variavel nome, novamente;
        nomeCompleto = PadronizarNomeCompletoUsuario(nomeCompleto);

        // console.log('Formatação/padronização dos parâmetros foi concluída: ' + nomeFormato);

        // Iniciar a barra de progresso, desabilitar botão e iniciar o loading;
        NProgress.start();
        DesabilitarElemento($btnCriarConta, true);
        IniciarLoadingFullScreen('');

        // Verificar no servidor (controller) se já existe esse e-mail;
        var requestChecarEmailDisponivel = $.ajax({
            url: '/Usuarios/ChecarEmailENomeUsuarioDisponivel',
            type: 'GET',
            data: { email: email, nomeUsuario: nomeUsuario },
            dataType: 'json',
            traditional: true,
            contentType: 'application/json; charset=utf-8'
        });

        // Inicio da verificação do login;
        requestChecarEmailDisponivel.done(function (resposta) {
            if (resposta.isDisponivel === false) {
                // Habilitar botão de criar conta, caso alguém já esteja usando o mesmo e-mail;
                NProgress.done();
                DesabilitarElemento($btnCriarConta, false);
                RemoverLoadingFullScreen();

                Aviso('error', resposta.msg + ' ' + Emoji('Aviso'), true, 4000);

                if (resposta.erro === "1") {
                    $txtEmail.focus();
                } else {
                    $txtUsuario.focus();
                }

                $txtSenha.val('');
                return;
            } else { // Caso não seja false = significa que o e-mail está disponível... 
                // Enviar a requisição pro servidor para criar o login;
                var requestCriarContaAjax = $.ajax({
                    url: '/Usuarios/CriarConta',
                    type: 'POST',
                    data: { nomeCompleto: nomeCompleto, email: email, nomeUsuario: nomeUsuario, senha: senha },
                    dataType: 'html'
                });

                // Sucesso;
                requestCriarContaAjax.done(function (idUsuarioCriado) {
                    // Logar diretamente;
                    Entrar(idUsuarioCriado, senha);
                });

                // Falha;
                requestCriarContaAjax.fail(function (jqXHR) {
                    // Habilitar botão de criar conta, caso dê algum bug;
                    NProgress.done();
                    DesabilitarElemento($btnCriarConta, false);
                    RemoverLoadingFullScreen();

                    Aviso('error', 'Parece que houve um erro ao registrar sua conta <br/> ' +
                        'Código do erro: ' + jqXHR.status + '', true, 4000);
                });
            }
        });
    });

    function PadronizarNomeCompletoUsuario(nome) {
        // Trim o nome, já que o usuário pode colocar espaços a mais;
        nome = nome.replace(/\s+/g, " ").trim();

        // Colocar letra maiúscula apenas nas primeiras letras, no nome;
        nome = nome.toLowerCase().replace(/\b[a-z]/g, function (letter) {
            return letter.toUpperCase();
        });

        // Colocar todas as palavras do nome em um array;
        var palavrasNome = nome.split(" ");

        // Todas palavras que tiverem < 4 caracteres, faça toLowerCase();
        // Por causa dos nomes "do, dos, da, das" e etc;
        var nomeFormatado = '';
        for (var i = 0; i < palavrasNome.length; i++) {
            if (i === 0) {
                if (palavrasNome[i].length < 4 && i > 0) {
                    nomeFormatado = palavrasNome[i].toLowerCase();
                } else {
                    nomeFormatado = palavrasNome[i];
                }
            } else {
                if (palavrasNome[i].length < 4 && i > 0) {
                    nomeFormatado = nomeFormatado + ' ' + palavrasNome[i].toLowerCase();
                } else {
                    nomeFormatado = nomeFormatado + ' ' + palavrasNome[i];
                }
            }
        }

        nome = nomeFormatado;

        return nome;
    }
});

