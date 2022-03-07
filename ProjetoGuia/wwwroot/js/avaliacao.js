// https://codepen.io/junior-the-vuer/pen/gOxyXKL
let range = document.querySelector('.range');
let slider = document.querySelector('.slider');
let gradientStop = document.querySelector('#gradient-stop');
let gradientGrey = document.querySelector('#gradient-grey');

let colorBad = '#ff5722';
let colorOk = '#3f51b5';
let colorGood = '#36d896';
let colorGreat = 'var(--cor-principal)';

slider.addEventListener('input', function () {
    // Mostrar #divAvaliacaoComentario e #divAvaliacaoSvg;
    $('#divAvaliacaoComentario, #divAvaliacaoSvg').removeClass('esconder').addClass('animate__animated animate__fadeIn animate__slow');

    // Exibir valor;
    var valor = CheckSliderValue(slider);
    ExibirNota(valor);
});

function CheckSliderValue(slider) {
    gradientGrey.setAttribute('offset', 100 - slider.value + '%');

    if (slider.value > 0 && slider.value <= 25) {
        range.closest('.row').classList.add('bad');
        range.closest('.row').classList.remove('ok', 'great', 'good');
        gradientStop.setAttribute('stop-color', colorBad);
    }

    if (slider.value > 25 && slider.value <= 50) {
        range.closest('.row').classList.add('ok');
        range.closest('.row').classList.remove('bad', 'great', 'good');
        gradientStop.setAttribute('stop-color', colorOk);
    }

    if (slider.value > 50 && slider.value <= 75) {
        range.closest('.row').classList.add('good');
        range.closest('.row').classList.remove('ok', 'great', 'bad');
        gradientStop.setAttribute('stop-color', colorGood);
    }

    if (slider.value > 75 && slider.value <= 100) {
        range.closest('.row').classList.add('great');
        range.closest('.row').classList.remove('ok', 'bad', 'good');
        gradientStop.setAttribute('stop-color', colorGreat);
    }

    return slider.value;
}

$(document).ready(function () {
    CheckSliderValue(slider);

    $('#btnCancelarAvaliacao').click(function () {
        LimparAvaliacao();
    });

    $('#btnEnviarAvaliacao').click(function () {
        var estabelecimentoId = $('#txtEstabelecimentoId').val();
        var valor = CheckSliderValue(slider);
        var nota = ExibirNota(valor);

        // Verificações;
        var comentario = $('#txtAvaliacao').val();

        if (!comentario) {
            Aviso('warning', 'Você deixou o campo de comentário em branco... ' + Emoji('aviso'), true, 4000);
            $('#txtAvaliacao').focus();
            return;
        }

        if (comentario.length < 15) {
            Aviso('warning', 'A sua avaliação está muito curta... ' + Emoji('aviso'), true, 4000);
            $('#txtAvaliacao').focus();
            return;
        }

        // Desabilitar elementos;
        DesabilitarElemento('#btnEnviarAvaliacao', true);
        DesabilitarElemento('#btnCancelarAvaliacao', true);
        DesabilitarElemento('#txtAvaliacao', true);

        // json;
        var estabelecimentoAvaliacao = {
            'EstabelecimentoId': estabelecimentoId,
            'Avaliacao': String(nota).replace('.', ','),
            'Comentario': comentario
        };

        // console.log(estabelecimentoAvaliacao);

        InformacoesUsuarioLogado(1).then(function (resposta) {
            var usuarioId = resposta;
            // console.log(usuarioId);

            // Se o usuário tiver logado;
            if (usuarioId !== -1) {
                // Request;
                var requestAvaliacao = $.ajax({
                    url: '/Estabelecimentos/AvaliarEstabelecimento',
                    type: 'POST',
                    data: { estabelecimentoAvaliacao: estabelecimentoAvaliacao },
                    dataType: 'html'
                });

                // Sucesso;
                requestAvaliacao.done(function (resposta) {
                    Aviso('success', 'Muito obrigado! A sua avaliação foi enviada com sucesso ' + Emoji('feliz'), true, 4000);
                    LimparAvaliacao();

                    // Exibir as avaliações novamente;
                    GetAvaliacoes();
                });
            } else {
                Aviso('error', 'Você precisa estar logado para deixar uma avaliação! ' + Emoji('triste'), true, 4000);
            }
        });
    });
});

function ExibirNota(valor) {
    var nota = valor / 20;

    $('#spanNotaAvaliacao').html('Nota ' + nota);
    return nota;
}

function LimparAvaliacao() {
    $('#txtAvaliacao').val('');
    $('#spanNotaAvaliacao').html('');
    $('#divAvaliacaoComentario, #divAvaliacaoSvg').addClass('esconder').removeClass('animate__animated animate__fadeIn animate__slow');
}
