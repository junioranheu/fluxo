$(document).ready(function () {
    // Toggle navbar;
    $(".navbar-burger").click(function () {
        $(".navbar-burger, .navbar-menu", $(this).closest('.navbar')).toggleClass("is-active");
    });

    // Remover os comentários no HTML - https://stackoverflow.com/questions/36285611/how-to-hide-html-comments;
    $('*').contents().each(function () {
        if (this.nodeType === Node.COMMENT_NODE) {
            $(this).remove();
        }
    });

    //// Desabilitar o click com o direito (não permitindo inspecionar elemento no browser);
    //$(document).bind("contextmenu", function (e) {
    //    e.preventDefault();
    //});

    //// Desabilitar apertar F12 para abrir o inspecionador de elementos no browser;
    //$(document).keydown(function (e) {
    //    if (e.which === 123) {
    //        return false;
    //    }
    //});
});

// Polling;
function Polling(func, wait, times) {
    var interv = function (w, t) {
        return function () {
            if (typeof t === "undefined" || t-- > 0) {
                setTimeout(interv, w);
                try {
                    func.call(null);
                }
                catch (e) {
                    t = 0;
                    throw e.toString();
                }
            }
        };
    }(wait, times);

    setTimeout(interv, wait);
}

// Toast (mensagem) -  https://www.jqueryscript.net/other/toast-notification-td-message.html;
function Aviso(tipo, mensagem, mostrarX, duracaoMilisegundos) {
    $.message({
        type: tipo, // info, success, error, warning
        text: mensagem,
        showClose: mostrarX,
        duration: duracaoMilisegundos,
        positon: 'bottom-left' // top-left, top-center (default), top-right, bottom-left, bottom-center, bottom-right
    });
}

function Emoji(tipo) {
    // Emojis: https://getemoji.com/
    var emoji = '';
    var i;

    //if (tipo === 'feliz') {
    //    var emojiFelizArray = [
    //        '🤗', '😋', '😉'
    //    ];

    //    i = Math.floor(Math.random() * emojiFelizArray.length);
    //    emoji = emojiFelizArray[i];
    //} else if (tipo === 'triste') {
    //    var emojiTristeArray = [
    //        '😟', '😕', '☹️', '🥴', '🤔'
    //    ];

    //    i = Math.floor(Math.random() * emojiTristeArray.length);
    //    emoji = emojiTristeArray[i];
    //} else if (tipo === 'aviso') {
    //    var emojiAvisoArray = [
    //        '🤔', '🤨', '🧐', '😬'
    //    ];

    //    i = Math.floor(Math.random() * emojiAvisoArray.length);
    //    emoji = emojiAvisoArray[i];
    //}

    return emoji;
}

// Polling para atualizar a data do usuário on-line;
$(document).ready(function () {
    InformacoesUsuarioLogado(1).then(function (resposta) {
        var usuarioId = resposta;
        // console.log(usuarioId);

        // Se o usuário tiver logado;
        if (usuarioId !== -1) {
            // Iniciar polling;
            Polling(function () {
                AtualizarDataOnline(usuarioId);
            }, 3000);
        }
    });
});

// Atualizar a data on-line;
function AtualizarDataOnline(usuarioRequisitadoId) {
    var requestAtualizarDataOnline = $.ajax({
        url: '/Usuarios/AtualizarDataOnline',
        type: 'GET',
        data: { usuarioRequisitadoId: usuarioRequisitadoId },
        dataType: 'json',
        traditional: true,
        contentType: 'application/json; charset=utf-8'
    });

    requestAtualizarDataOnline.done(function (resposta) {
        //if (resposta.isOk === '1') {
        //    Aviso('success', 'Última hora on-line atualizada ' + Emoji('feliz'), true, 5000);
        //} else {
        //    Aviso('error', 'Erro ao atualizar a última hora on-line ' + Emoji('triste'), true, 5000);
        //}
    });
}

// A função Entrar está aqui pois é chamada em vários locais (Criar conta e-mail, entrar...)
// Basta passar os parâmetros corretos;
// Mudança feita, usando o cookie_voltar_login;
function Entrar(idUsuario, senha) {
    // Iniciar a barra de progresso;
    NProgress.start();

    // Logar, de fato;
    var requestEntrar = $.ajax({
        url: "/Usuarios/Login",
        type: "GET",
        data: { idUsuario: idUsuario, senha: senha },
        dataType: "json",
        traditional: true,
        contentType: "application/json; charset=utf-8"
    });

    requestEntrar.done(function (resposta) {
        if (resposta.usuarioLogado === true) {
            // Usuário já logado;
            var m = 'Parece que este usuário já está logado no sistema!<br/>Tente novamente mais tarde';
            Aviso('error', m + ' ' + Emoji('triste'), true, 5000);
            RemoverLoadingFullScreen();
            DesabilitarElemento('#btnEntrar', false);

            $('#txtSenha').val('');
            NProgress.done();
        } else if (resposta.isAtivo === false) {
            // Usuário desativado;
            m = 'Parece que este usuário está desativado!';
            Aviso('error', m + ' ' + Emoji('triste'), true, 5000);
            RemoverLoadingFullScreen();
            DesabilitarElemento('#btnEntrar', false);

            $('#txtSenha').val('');
            NProgress.done();
        }
        else {
            // Se tiver ok;
            // Se já existir o cookie cookie_voltar_login, volte para a url do cookie;
            if (Cookies.get('cookie_voltar_login')) {
                var url = Cookies.get('cookie_voltar_login');

                // Remover o cookie em questão;
                Cookies.remove('cookie_voltar_login');

                // Redirecionar para a página anterior do aviso;
                window.location.href = url;
            } else {
                // Se não tiver o cookie_voltar_login, vá para a tela inicial;
                window.location.href = "/";
            }

            NProgress.done();
        }
    });

    //requestEntrar.fail(function (xhr, textStatus, error) {
    //    console.log(xhr.statusText);
    //    console.log(textStatus);
    //    console.log(error);
    //});
}

// Exibir loading;
function IniciarLoadingFullScreen(texto) {
    // https://www.jqueryscript.net/loading/Fullscreen-Loading-Modal-Indicator-Plugin-For-jQuery-loadingModal.html
    $('body').loadingModal({
        position: 'auto',
        text: texto,
        color: 'var(--cor-principal)',
        opacity: '0.8',
        backgroundColor: 'rgb(0,0,0)',
        animation: 'cubeGrid'
        // animation: 'wanderingCubes'
    });
}

// Remover loading;
function RemoverLoadingFullScreen() {
    $('body').loadingModal('destroy');
}

// Habilitar ou desabilitar elemento;
function DesabilitarElemento(elemento, isDesabilitar) {
    $(elemento).attr('disabled', isDesabilitar);
}

function ChecarSenha(senha) {
    var number = /([0-9])/;
    var alphabets = /([a-zA-Z])/;
    var special_characters = /([~,!,@,#,$,%,^,&,*,-,_,+,=,?,>,<])/;

    if (senha.length < 6) {
        Aviso('warning', 'Sua senha deve ter pelo menos 06 caracteres ' + Emoji('aviso'), true, 4000);
        document.getElementById("txtSenha").focus();
        return false;
    } else {
        if (senha.match(number) && senha.match(alphabets)) { // && senha.match(special_characters)
            // Aviso('success', 'Sua senha é bem forte!', true, 4000);
            return true;
        } else {
            Aviso('warning', 'Sua senha não é forte o suficiente ' + Emoji('aviso') + ' <br/>' +
                'Lembre-se de usar: letras e números!', true, 6000);
            document.getElementById("txtSenha").focus();
            return false;
        }
    }
}

// Logout;
$(document).ready(function () {
    $("#btnLogout, #btnLogoutMobile").click(function () {
        IniciarLoadingFullScreen();

        setTimeout(function () {
            var requestLogout = $.ajax({
                url: "/Usuarios/Logout",
                type: "GET",
                dataType: "json",
                traditional: true,
                contentType: "application/json; charset=utf-8"
            });

            requestLogout.done(function (resposta) {
                window.location = "/";
            });
        }, 800);
    });
});

// Função para mostrar acesso negado ao usuário tentar realizar algo sem estar logado;
function AcessoNegado(fazer) {
    // Array dos opasss;
    var opaArray = [
        'Opa!',
        'Epa!',
        'Ei!',
        'Vixi!',
        'Pera aeeeê',
        'Parado, agora!'
    ];

    // Pega algum aleatório;
    var numeroAleatorio = Math.floor(Math.random() * opaArray.length);
    var opa = opaArray[numeroAleatorio];

    $.alert('' + opa + ' <br/><br/>' +
        'Você tem que entrar na sua conta para poder ' + fazer + '! ' + Emoji('aviso') + ' <br/><br/>' +
        'Entre clicando ' +
        '<a href="/entrar" id="btnEntrarAcesso" class="cor-principal">aqui</a>');

    // Ao clicar no btnEntrarAcesso para entrar, set o valor da url atual no cookie_voltar_login;
    $('#btnEntrarAcesso').click(function () {
        var urlAnteriorLogin = window.location.href;
        Cookies.set('cookie_voltar_login', urlAnteriorLogin);
    });
}

function InformacoesUsuarioLogado(info) {
    // Usar promise: https://www.codegrepper.com/code-examples/javascript/jquery+ajax+not+waiting+for+response
    return new Promise(function (resolve, reject) {
        var resposta = '-1';

        var requestInformacoesUsuarioLogado = $.ajax({
            url: "/Usuarios/TrazerInformacoesUsuarioLogado",
            type: "GET",
            // data: { xxx: xxx },
            dataType: "json",
            traditional: true,
            contentType: "application/json; charset=utf-8"
        });

        requestInformacoesUsuarioLogado.done(function (resposta) {
            // console.log(resposta);
            var usuarioId = resposta.usuario.usuarioId; // 1;
            var nomeCompleto = resposta.usuario.nomeCompleto; // 2;
            var usuarioTipoId = resposta.usuario.usuarioTipoId; // 3;
            var nomeUsuarioSistema = resposta.usuario.nomeUsuarioSistema; // 4;
            var foto = resposta.usuario.foto; // 5;

            if (info === 1) {
                resposta = usuarioId;
            } else if (info === 2) {
                resposta = nomeCompleto
            }
            else if (info === 3) {
                resposta = usuarioTipoId
            }
            else if (info === 4) {
                resposta = nomeUsuarioSistema
            }
            else if (info === 5) {
                resposta = foto
            }

            resolve(resposta);
        });
    });
}

// Função para exibir/remover loading dependendo se todas as imagens foram carregadas;
function LoadingEsperarTodasAsImagensCarregarem() {
    // Iniciar loading;
    IniciarLoadingFullScreen();

    // https://stackoverflow.com/questions/11071314/javascript-execute-after-all-images-have-loaded
    var imgs = document.images,
        len = imgs.length,
        counter = 0;

    [].forEach.call(imgs, function (img) {
        if (img.complete) {
            incrementCounter();
        }
        else {
            img.addEventListener('load', incrementCounter, false);
        }
    });

    function incrementCounter() {
        counter++;
        // console.log(counter + ' de ' + len);

        if (counter === len) {
            // console.log('Todas as imagens foram carregadas');

            // Finalizar loading;
            RemoverLoadingFullScreen();
        }
    }

    // Caso demore mais de tempoLimite, remova de qualquer forma;
    var tempoLimite = 2000;
    setTimeout(function () {
        RemoverLoadingFullScreen();
    }, tempoLimite);
}

// Função para verificar se todos os campos de um json estão preenchidos;
function VerificarPreenchimentoJson(dados) {
    var isPreenchido = true;
    var mensagem = '';

    $.each(dados, function (index, value) {
        const keys = Object.keys(value);
        for (let i = 0; i < keys.length; i++) {
            const k = keys[i];
            const valores = value[k];
            // console.log(valores);

            for (var key in valores) {
                // console.log(key + " -> " + valores[key]);
                if (!valores[key]) {
                    // var m = 'O campo ' + key + ' não foi preenchido! ';
                    var m = key + '\\';
                    // console.log(m);

                    isPreenchido = false;
                    mensagem += m;
                }
            }
        }
    });

    var retorno = {
        isPreenchido: isPreenchido,
        mensagem: mensagem
    };

    return retorno;

    // COMO USAR:
    /*
             // Verificação;
        var retornoVerificacaoJson = VerificarPreenchimentoJson(dados);
        if (retornoVerificacaoJson.isPreenchido === false) {
            var campos = retornoVerificacaoJson.mensagem;
            campos = campos.replaceAll('\\', '<br/>');

            Aviso('warning',
                'Existem campos que não foram preenchidos ainda ' + Emoji('triste') +
                '<br/><br/>Campos: <br/>' + campos + '', true, 4000);
            return false;
        }
     */
}

// Verificação de e-mail #2;
function ChecarEmail(email) {
    //var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    var regex = /^([a-zA-Z0-9_.\-+])+@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}

function SegundosParaDHMS(seconds) {
    seconds = Number(seconds);
    var d = Math.floor(seconds / (3600 * 24));
    var h = Math.floor(seconds % (3600 * 24) / 3600);
    var m = Math.floor(seconds % 3600 / 60);
    var s = Math.floor(seconds % 60);

    var dDisplay = d > 0 ? d + (d == 1 ? ' dia' : ' dias') : '';
    var hDisplay = h > 0 ? h + (h == 1 ? ' hora' : ' horas') : '';
    var mDisplay = m > 0 ? m + (m == 1 ? ' minuto' : ' minutos') : '';
    var sDisplay = s > 0 ? s + (s == 1 ? ' segundo' : ' segundos') : '';

    var retorno = '';
    if (dDisplay) {
        retorno = dDisplay;
    } else if (hDisplay) {
        retorno = hDisplay;
    } else if (mDisplay) {
        retorno = mDisplay;
    } else if (sDisplay) {
        retorno = sDisplay;
    }

    // return dDisplay + hDisplay + mDisplay + sDisplay;
    return 'há ' + retorno;
}
