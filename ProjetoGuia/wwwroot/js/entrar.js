// Definir os elementos nas variáveis;
var $txtUsuario = $('#txtUsuario');
var $txtSenha = $('#txtSenha');
var $btnEntrar = $('#btnEntrar');

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

$(document).ready(function () {
    // Focar no input;
    $txtUsuario.focus();

    // Verificações a respeito do login;
    // Se o usuário tiver nos inputs da de email ou senha do entrar, simule o click no botão;
    $('#txtUsuario, #txtSenha').keypress(function (event) {
        if (event.which === 13 || event.keyCode === 13) {
            $btnEntrar.click();
        }
    });

    // Não permitir utilizar espaço no nome de usuário;
    $txtUsuario.keypress(function (e) {
        if (e.which === 32) {
            return false;
        }
    });

    $btnEntrar.click(function () {
        // Pegar os valores dos campos;
        var usuario = $txtUsuario.val();
        var senha = $txtSenha.val();

        // Verificar se os campos estão preenchidos;
        if (!usuario) {
            Aviso('warning', 'Insira o seu nome de usuário ou e-mail para entrar ' + Emoji('aviso'), true, 4000);
            $txtUsuario.focus();
            return;
        }

        if (!senha) {
            Aviso('warning', 'Insira a sua senha para entrar ' + Emoji('aviso'), true, 4000);
            $txtSenha.focus();
            return;
        }

        // Iniciar a barra de progresso, desabilitar botão e iniciar o loading;
        NProgress.start();
        DesabilitarElemento($btnEntrar, true);
        IniciarLoadingFullScreen('');

        // Verificar se o login e a senha estão corretos;
        var requestChecarUsuario = $.ajax({
            url: '/Usuarios/VerificarEmailSenha',
            type: 'GET',
            data: { nomeUsuarioSistema: usuario, senha: senha },
            dataType: 'json',
            traditional: true,
            contentType: 'application/json; charset=utf-8'
        });

        // Inicio da verificação do login;
        requestChecarUsuario.done(function (resposta) {
            if (resposta.isExiste === false) {
                // Finalizar a barra de progresso, habilitar o botão e remover o loading;
                setTimeout(function () {
                    Aviso('error', 'Parece que o nome de usuário/e-mail e/ou senha estão incorretos ' + Emoji('triste'), true, 4000);
                    $txtSenha.val('');
                    $txtUsuario.focus();

                    NProgress.done();
                    DesabilitarElemento($btnEntrar, false);
                    RemoverLoadingFullScreen();
                    return;
                }, 100);
            } else {
                // Logar;
                Entrar(resposta.idUsuario, senha);
            }
        });
    });
});

