
// Definir os elementos nas variáveis;
var $txtNome = $('#txtNome');
var $txtEmail = $('#txtEmail');
var $txtNomeUsuario = $('#txtNomeUsuario');
var $txtSenha = $('#txtSenha');
var $txtCPF = $('#txtCPF');
var $txtTelefone = $('#txtTelefone');
var $txtDataAniversario = $('#txtDataAniversario');
var $txtCEP = $('#txtCEP');
var $txtNumeroResidencia = $('#txtNumeroResidencia');
var $txtRua = $('#txtRua');
var $txtBairro = $('#txtBairro');
var $txtEstado = $('#txtEstado');
var $txtCidade = $('#txtCidade');

var $liDadosApp = $('#liDadosApp');
var $liDadosPessoais = $('#liDadosPessoais');
var $liSobre = $('#liSobre');
var $btnSalvarAlteracao = $('#btnSalvarAlteracao');

// ViaCEP - https://viacep.com.br/exemplo/jquery/;
function LimpaFormularioCep() {
    // Limpa valores do formulário de cep.
    $txtRua.val('');
    $txtBairro.val('');
    $txtCidade.val('');
    $txtEstado.val('');
}

// Quando o campo CEP perde o foco;
$txtCEP.blur(function () {
    BuscarCEP($(this).val());
});

// Quando o campo de CEP está com 10 caracteres (equivalente a um CEP (12605-110, ex));
$txtCEP.on('input', function () {
    var qtd = $(this).val().length;
    // console.log(qtd);

    if (qtd === 9 || qtd === 0) {
        BuscarCEP($(this).val());
    }
});

function BuscarCEP(cep0) {
    // Nova variável "cep" somente com dígitos;
    var cep = cep0.replace(/\D/g, '');

    // Verifica se campo cep possui valor informado;
    if (cep) {
        // Expressão regular para validar o CEP;
        var validacep = /^[0-9]{8}$/;

        // Valida o formato do CEP;
        if (validacep.test(cep)) {
            // Consulta o webservice viacep.com.br/
            $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {
                // console.log(dados);

                if (!("erro" in dados)) {
                    // Atualiza os campos com os valores da consulta;
                    $txtRua.val(dados.logradouro);
                    $txtBairro.val(dados.bairro);
                    $txtCidade.val(dados.localidade);
                    $txtEstado.val(dados.uf);
                    // $("#selectEstado option:contains(" + dados.uf + ")").attr('selected', 'selected');
                }
                else {
                    // CEP pesquisado não foi encontrado;
                    LimpaFormularioCep();
                    Aviso('error', 'CEP não encontrado ' + Emoji('triste'), true, 4000);
                }
            });
        }
        else {
            // CEP é inválido;
            LimpaFormularioCep();
            Aviso('error', 'Formato de CEP inválido ' + Emoji('triste'), true, 4000);
        }
    }
    else {
        // CEP sem valor, limpa formulário;
        LimpaFormularioCep();
        // Aviso("Opa!", "O que houve com seu CEP!? 😕", 3000);
    }
}

function ValidarCPF(strCPF) {
    var soma;
    var resto;
    soma = 0;

    if (strCPF === "00000000000") return false;

    for (i = 1; i <= 9; i++) soma = soma + parseInt(strCPF.substring(i - 1, i)) * (11 - i);
    resto = (soma * 10) % 11;

    if ((resto === 10) || (resto === 11)) resto = 0;
    if (resto !== parseInt(strCPF.substring(9, 10))) return false;

    soma = 0;
    for (i = 1; i <= 10; i++) soma = soma + parseInt(strCPF.substring(i - 1, i)) * (12 - i);
    resto = (soma * 10) % 11;

    if ((resto === 10) || (resto === 11)) resto = 0;
    if (resto !== parseInt(strCPF.substring(10, 11))) return false;

    return true;
}

// Tabs;
$(document).ready(function () {
    $liDadosApp.addClass('is-active');
    $('#divDadosApp').removeClass('esconder');

    $liDadosApp.click(function () {
        $liDadosApp.addClass('is-active');
        $('#divDadosApp').removeClass('esconder');

        $liDadosPessoais.removeClass('is-active');
        $('#divDadosPessoais').addClass('esconder');

        // $liSobre.removeClass('is-active');
        // $('#divSobre').addClass('esconder');
    });

    $liDadosPessoais.click(function () {
        $liDadosPessoais.addClass('is-active');
        $('#divDadosPessoais').removeClass('esconder');

        $liDadosApp.removeClass('is-active');
        $('#divDadosApp').addClass('esconder');

        // $liSobre.removeClass('is-active');
        // $('#divSobre').addClass('esconder');
    });

    $liSobre.click(function () {
        $liDadosPessoais.removeClass('is-active');
        $('#divDadosPessoais').addClass('esconder');

        $liDadosApp.removeClass('is-active');
        $('#divDadosApp').addClass('esconder');

        $liSobre.addClass('is-active');
        $('#divSobre').removeClass('esconder');
    });
});

function Mascaras() {
    // Formatar os campos;
    $txtDataAniversario.mask('00/00/0000', { clearIfNotMatch: true });
    $txtCPF.mask('000.000.000-00', { reverse: true, clearIfNotMatch: true });
    $txtCEP.mask('00000-000', { clearIfNotMatch: true });
    $txtNumeroResidencia.mask('0#', { clearIfNotMatch: true });

    $txtTelefone.on('click', function (e) {
        $txtTelefone.mask('(00) 90000-0000');
    });

    $txtTelefone.on('focusout', function (e) {
        if ($(this).val().length === 15) {
            $txtTelefone.mask('(00) 90000-0000');
        } else if ($(this).val().length < 15 && $(this).val().length > 13) {
            $txtTelefone.mask('(00) 0000-0000');
        } else {
            $txtTelefone.val('');
        }
    });

    // Permitir inserir apenas letras e espaços no nome completo;
    $txtNome.on('keypress', function (e) {
        var regex = new RegExp("^[a-zA-Z ]*$");

        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });

    // Permitir inserir apenas letras e números no nome de usuário;
    $txtNomeUsuario.on('keypress', function (e) {
        var regex = new RegExp("^[a-zA-Z1-9]*$");

        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }

        e.preventDefault();
        return false;
    });
}

function VerificarCampos() {
    var nome = $txtNome.val();
    var email = $txtEmail.val();
    var nomeUsuario = $txtNomeUsuario.val();
    var senha = $txtSenha.val();
    var cpf = $txtCPF.val();
    var telefone = $txtTelefone.val();
    var dataAniversario = $txtDataAniversario.val();
    var genero = $('input[type="radio"][name="rbGenero"]:checked').val();
    var cep = $txtCEP.val();
    var numeroResidencia = $txtNumeroResidencia.val();
    var rua = $txtRua.val();
    var bairro = $txtBairro.val();
    var estado = $txtEstado.val();
    var cidade = $txtCidade.val();

    // Verificação do nome #1: nome preenchido?;
    if (!nome) {
        Aviso('warning', 'Parece que você esqueceu de colocar o seu nome ' + Emoji('aviso'), true, 4000);
        $liDadosApp.trigger('click');
        $txtNome.focus();
        return false;
    }

    // Verificação do nome #2: pelo menos 03 caracteres?;
    if (nome.length < 3) {
        Aviso('warning', 'Seu nome não pode ter menos de 03 caracteres ' + Emoji('aviso'), true, 4000);
        $liDadosApp.trigger('click');
        $txtNome.focus();
        return false;
    }

    // Verificação do nome #3: se existe pelo menos um espaço (dois nomes), false = não;
    var reg = new RegExp("(\\w+)(\\s+)(\\w+)");
    if (reg.test(nome) === false) {
        Aviso('warning', nome + ' é um belo nome, mas cadê seu sobrenome? ' + Emoji('aviso'), true, 4000);
        $liDadosApp.trigger('click');
        $txtNome.focus();
        return false;
    }

    // Verificação de e-mail #1: e-mail preenchido?;
    if (!email) {
        Aviso('warning', 'Parece que você esqueceu de colocar o seu e-mail ' + Emoji('Aviso'), true, 4000);
        $liDadosApp.trigger('click');
        $txtEmail.focus();
        return false;
    }

    // Verificação de e-mail #2: e-mail válido?;
    if (ChecarEmail(email) === false) {
        Aviso('error', 'Parece que esse e-mail não é válido... ' + Emoji('Aviso'), true, 4000);
        $liDadosApp.trigger('click');
        $txtEmail.focus();
        return false;
    }

    // Verificação de nome de usuário #1: nome de usuário preenchido?;
    if (!nomeUsuario) {
        Aviso('warning', 'Parece que você esqueceu de colocar um nome de usuário (apelido que será utilizado no sistema) ' + Emoji('Aviso'), true, 4000);
        $liDadosApp.trigger('click');
        $txtNomeUsuario.focus();
        return false;
    }

    // Verificação de nome de usuário #2: pelo menos 03 caracteres?;
    if (nomeUsuario.length > 20 || nomeUsuario.length < 4) {
        Aviso('warning', 'O nome de usuário não pode ter não pode ter menos de 4 e nem mais de 10 caracteres, e agora está com ' + nomeUsuario.length + '! ' + Emoji('Aviso'), true, 7000);
        $liDadosApp.trigger('click');
        $txtNomeUsuario.focus();
        return false;
    }

    // Verificação de senha #1: preenchido?;
    // if (!senha) {
    //     Aviso('warning', 'Parece que você esqueceu de colocar sua senha ' + Emoji('Aviso'), true, 4000);
    //     $liDadosApp.trigger('click');
    //     $txtSenha.focus();
    //     return false;
    // }

    // Verificação da senha #2 (fica no site.js): realizar uma série de verificações, se alguma retornar falso, aborte;
    if (senha) {
        if (ChecarSenha(senha) === false) {
            return false;
        }
    }

    // Verificação do CPF #1: preenchido?;
    if (!cpf) {
        Aviso('warning', 'Parece que você esqueceu de colocar seu CPF ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtCPF.focus();
        return false;
    }

    // Verificação do CPF #2: válido?;
    if (ValidarCPF(cpf.replace(/\D/g, '')) === false) {
        Aviso('warning', 'Parece que esse CPF não é válido ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtCPF.focus();
        return false;
    }

    // Verificação do telefone #1: preenchido?;
    if (!telefone) {
        Aviso('warning', 'Parece que você esqueceu de colocar seu número de telefone ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtTelefone.focus();
        return false;
    }

    // Verificação do telefone #2: válido?;
    if (telefone.length < 11) {
        Aviso('warning', 'Parece que tem algo de errado com o formato do seu número de telefone ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtTelefone.focus();
        return false;
    }

    // Verificação da data de aniversário #1;
    if (!moment(dataAniversario, 'DD/MM/YYYY', true).isValid() || dataAniversario === '01/01/0001') {
        Aviso('warning', 'Parece que a data inserida não é válida ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtDataAniversario.focus();
        return false;
    }

    // Verificação do gênero #1: preenchido?;
    if (!genero) {
        Aviso('warning', 'Parece que você esqueceu de selecionar seu genêro ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        return false;
    }

    // Verificação do cep #1: preenchido?;
    if (!cep) {
        Aviso('warning', 'Parece que você esqueceu de colocar seu CEP ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtCEP.focus();
        return false;
    }

    // Verificação do cep #2: válido?;
    if (cep.length < 9) {
        Aviso('warning', 'Parece que tem algo de errado com o formato do seu CEP ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtCEP.focus();
        return false;
    }

    // Verificação do numeroResidencia #1: preenchido?;
    if (!numeroResidencia) {
        Aviso('warning', 'Parece que você esqueceu de colocar o número da sua residência ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtNumeroResidencia.focus();
        return false;
    }

    // Verificação do estado #1: preenchido?;
    if (!estado) {
        Aviso('warning', 'Parece que você esqueceu de colocar seu estado! Ajuste seu CEP ' + Emoji('aviso'), true, 4000);
        $liDadosPessoais.trigger('click');
        $txtCEP.focus();
        return false;
    }

    // json;
    var dados = {
        'nomeCompleto': nome,
        'email': email,
        'nomeUsuarioSistema': nomeUsuario,
        'senha': senha,
        'foto': '',

        'usuariosInformacoes':
        {
            'genero': genero,
            'dataAniversario': dataAniversario,
            'cpf': cpf,
            'telefone': telefone,
            'rua': rua,
            'numeroResidencia': numeroResidencia,
            'cep': cep,
            'bairro': bairro,
            'cidades': {
                'nome': cidade,
                'estados': {
                    'sigla': estado
                },
            },
        }
    };

    return dados;
}

// Foto de perfil;
var arquivoFotoPerfil;
$(document).ready(function () {
    $('#div-imagem-upload').click(function () {
        $('#inputFileUpload').trigger('click');
    });

    $('#inputFileUpload').on('change', function (e) {
        // Imagem selecionada;
        arquivoFotoPerfil = $(this)[0].files[0];

        // Exibir imagem selecionada;
        var tmppath = URL.createObjectURL(arquivoFotoPerfil); // Converter a imagem selecionada para exibi-la: https://stackoverflow.com/questions/36030755/show-a-preview-image-of-an-html5-input-type-file-selected-image-file;
        $('#div-imagem-upload').css('background-image', 'url(' + tmppath + ')');

        // Aviso;
        var tamanhoMBs = (arquivoFotoPerfil.size / (1024 * 1024)).toFixed(2);
        Aviso('info', 'Nova imagem selecionada:<br/>' +
            '' + arquivoFotoPerfil.name + ' (' + tamanhoMBs + ' MBs)',
            true, 4000);
    });
});

// #btnSalvarAlteracao;
$(document).ready(function () {
    // Máscaras;
    Mascaras();

    $btnSalvarAlteracao.click(function () {
        DesabilitarElemento($btnSalvarAlteracao, true);

        // Verificar se os campos foram preenchidos;
        var dados = VerificarCampos();
        // console.log(dados);
        if (!dados) {
            DesabilitarElemento($btnSalvarAlteracao, false);
            return false;
        }

        // Como é necessário passar parâmetros que são string e IFormFile,
        // Os dados serão passados dando append em um formData (https://stackoverflow.com/questions/46754951/how-to-receive-a-file-and-an-integer-sent-through-an-ajax-request-in-action-meth);
        var formData = new FormData();
        formData.append('cidadeNome', dados.usuariosInformacoes.cidades.nome);
        formData.append('estadoSigla', dados.usuariosInformacoes.cidades.estados.sigla);
        formData.append('nomeCompleto', dados.nomeCompleto);
        formData.append('email', dados.email);
        formData.append('nomeUsuarioSistema', dados.nomeUsuarioSistema);
        formData.append('senha', dados.senha);
        formData.append('CPF', dados.usuariosInformacoes.cpf);
        formData.append('telefone', dados.usuariosInformacoes.telefone);
        formData.append('dataAniversario', dados.usuariosInformacoes.dataAniversario);
        formData.append('genero', dados.usuariosInformacoes.genero);
        formData.append('CEP', dados.usuariosInformacoes.cep);
        formData.append('numeroResidencia', dados.usuariosInformacoes.numeroResidencia);
        formData.append('rua', dados.usuariosInformacoes.rua);
        formData.append('bairro', dados.usuariosInformacoes.bairro);

        // Verificar se o usuário tiver upando foto de perfil;
        if (typeof arquivoFotoPerfil !== 'undefined') {
            formData.append('arquivoFotoPerfil', arquivoFotoPerfil);
        }

        // Se estiver tudo ok, salve as informações;
        NProgress.start();

        // Enviar a requisição pro servidor;
        var requestSalvar = $.ajax({
            url: '/Usuarios/AtualizarPerfil',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            dataType: 'json'
        });

        // Sucesso;
        requestSalvar.done(function (data) {
            if (data.resultado === 'true') {
                Aviso('success', 'Alterações salvas com sucesso! ' + Emoji('feliz'), true, 4000);
                DesabilitarElemento($btnSalvarAlteracao, false);

                // Se a imagem foi alterada, force um Control F5;
                if (typeof arquivoFotoPerfil !== 'undefined') {
                    //Aviso('info', 'Você alterou algumas coisas importantes... a página será atualizada! ' + Emoji('feliz'), true, 4000);

                    //setTimeout(function () {
                    //    window.location.reload(true);
                    //}, 3000);

                    //window.location.href = "/";
                }
            } else {
                Aviso('error', data.resultado, true, 4000);
                DesabilitarElemento($btnSalvarAlteracao, false);
            }

            NProgress.done();
        });

        // Falha;
        requestSalvar.fail(function (jqXHR) {
            // Habilitar botão de criar conta, caso dê algum bug;
            NProgress.done();
            DesabilitarElemento($btnSalvarAlteracao, false);

            Aviso('error', 'Parece que houve um erro ao salvar as alterações ' + Emoji('triste') + '<br/>' +
                'Código do erro: ' + jqXHR.status, true, 4000);
        });
    });
});