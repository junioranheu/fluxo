$(document).ready(function () {
    // Remover bug do "X" do alert nessa página; 
    // $('.icon[p-id="2131"]').remove();

    // Ao clicar em um post;
    $(".image-wrapper").click(function () {
        var titulo = $(this).find('.image-name').html();
        var conteudo = $(this).find('.image-subtext').html();
        var midia = $(this).find('img').attr('src');

        $.ajax({
            url: '/Estabelecimentos/ModalPost',
            type: 'POST',
            // data: { xxx: 'xxx' },
            cache: false,
            dataType: 'html',
            success: function (data) {
                // console.log(data);
                $('body').append(data);

                // Atribuir os valores correto ao modal;
                $('#spanTituloModal').html(titulo);
                $('#spanConteudoModal').html(conteudo);

                if (midia && (midia.indexOf('cinza') === -1 && midia.indexOf('smile') === -1)) {
                    $('#imgMidiaModal').attr('src', midia).show();
                }
            },
            error: function (data) {

            }
        });
    });

    // Ao clicar fora do post;
    $(document).on('click', '.modal-background, .modal-close', function () {
        $('#modalPost').remove();
    });

    // Pegar as avaliações do estabelecimento;
    GetAvaliacoes();
});

function GetAvaliacoes() {
    var estabelecimentoId = $('#txtEstabelecimentoId').val();

    // Request;
    var requestAvaliacao = $.ajax({
        url: '/Estabelecimentos/EstabelecimentoAvaliacoes',
        type: 'GET',
        data: { estabelecimentoId: estabelecimentoId },
        dataType: 'json',
        traditional: true,
        contentType: 'application/json; charset=utf-8'
    });

    // Sucesso;
    requestAvaliacao.done(function (dados) {
        // console.log(dados);

        if (dados.avaliacoes.length === 0) {
            var str = `<div id="divSemAvaliacao">Não existem avaliações para esse estabelecimento ainda</div>`;
            $('#divAvaliacoes').append(str);
        } else {
            $('#divSemAvaliacao').remove();

            $.each(dados.avaliacoes, function (index, data) {
                // console.log(data);
                var id = data.estabelecimentoAvaliacaoId;
                var usuario = data.usuarios.nomeUsuarioSistema;
                var nota = data.avaliacao;
                var comentario = data.comentario;
                var dataAvaliacao = data.data;

                // Data da avaliação;
                var currentDate = moment();
                var diferencaAgoraEDataComentario = moment.duration(currentDate.diff(dataAvaliacao));
                var diferencaEmSegundos = diferencaAgoraEDataComentario.asSeconds();
                var dataAvaliacaoFinal = SegundosParaDHMS(diferencaEmSegundos);

                // Link do perfil do avaliador;
                var urlPerfil = '/perfil/@' + usuario;

                var str = `<div class="comment-wrapper" id="divAvaliacao${id}">
                                <div class="container">
                                    <div class="center-blockxxx">
                                        <div class="media-comment">
                                            <!-- <a class="avatar-content" href="javascript://"><img class="avatar has-image-centered" src="https://randomuser.me/api/portraits/men/77.jpg" /></a> -->

                                            <div class="media-content">
                                                <div class="media-comment-body">
                                                    <div class="media-option">
                                                        <a class="ripple-grow" href="javascript://" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                            <svg class="ripple-icon" width="28" height="28" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" viewBox="0 0 24 24">
                                                                <g fill="currentColor">
                                                                    <circle cx="5" cy="12" r="2"></circle>
                                                                    <circle cx="12" cy="12" r="2"></circle>
                                                                    <circle cx="19" cy="12" r="2"></circle>
                                                                </g>
                                                            </svg>
                                                        </a>
                                                    </div>

                                                    <div class="media-comment-data-person">
                                                        <a class="media-comment-name" href="${urlPerfil}" target="_blank">
                                                             @${usuario}
                                                        </a>

                                                        <span class="text-muted">Nota ${nota}, ${dataAvaliacaoFinal}</span>
                                                    </div>

                                                    <div class="media-comment-text">${comentario}</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`;

                if (!$(`#divAvaliacao${id}`).length) {
                    $('#divAvaliacoes').append(str);
                }
            });
        }
    });
}

