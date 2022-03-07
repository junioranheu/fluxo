$(document).ready(function () {
    // LoadingEsperarTodasAsImagensCarregarem();

    // Ao iniciar;
    FiltrarTiposEstabelecimentos();

    // Filtrar clicando pelas categorias;
    $('.categoria').not('#opcaoMostrarTodasCategorias').click(function () {
        // Desativar o "mostrar todos", em qualquer caso;
        $('#opcaoMostrarTodasCategorias').attr('data-is-selecionado', '0');
        $('#opcaoMostrarTodasCategorias').find('.access-icon').css({ "backgroundColor": "var(--light-font)", "transition": "background-color 0.5s ease" });

        // Pegar se a opção clicada é selecionada ou não;
        var isSelecionado = $(this).attr('data-is-selecionado');

        // Setar ou tirar valor no elemento clicado/desclicado;
        if (isSelecionado === '0') {
            $(this).attr('data-is-selecionado', '1');
            $(this).find('.access-icon').css({ "backgroundColor": "var(--cor-principal)", "transition": "background-color 0.5s ease" });
        } else {
            $(this).attr('data-is-selecionado', '0');
            $(this).find('.access-icon').css({ "backgroundColor": "var(--light-font)", "transition": "background-color 0.5s ease" });
        }

        // Filtrar, de fato;
        FiltrarTiposEstabelecimentos();

        // Scroll;
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#divCategorias").offset().top
        }, 1000);
    });

    // Filtrar ao digitar no campo de pesquisa;
    $('#txtFiltro').on('keyup', function () {
        FiltrarTiposEstabelecimentos();
    });

    // Se clicar no #opcaoMostrarTodasCategorias, desative todos, e ative ele;
    $('#opcaoMostrarTodasCategorias').click(function () {
        $('#opcaoMostrarTodasCategorias').attr('data-is-selecionado', '1');
        $('#opcaoMostrarTodasCategorias').find('.access-icon').css({ "backgroundColor": "var(--cor-principal)", "transition": "background-color 0.5s ease" });

        $('.categoria').not('#opcaoMostrarTodasCategorias').each(function () {
            $(this).attr('data-is-selecionado', '0');
            $(this).find('.access-icon').css({ "backgroundColor": "var(--light-font)", "transition": "background-color 0.5s ease" });
        });

        // Filtrar novamente;
        FiltrarTiposEstabelecimentos();
    });
});

function FiltrarTiposEstabelecimentos() {
    // Apagar a possível mensagem que é criada na função MensagemTiposEstabelecimentos();
    $('#imgMensagemTiposEstabelecimentos').remove();
    $('#spanMensagemTiposEstabelecimentos').remove();

    // Pegar o que foi escrito no #txtFiltro e instanciar a lista categoriasSelecionadas;
    var busca = $('#txtFiltro').val().toLowerCase();
    var categoriasSelecionadas = [];

    // Pegar todos os ids das categorias selecionadas;
    $('.categoria').not('#opcaoMostrarTodasCategorias').each(function () {
        var isSelecionado = $(this).attr('data-is-selecionado');

        if (isSelecionado === '1') {
            var estabelecimentoCategoriaId = $(this).attr('data-id');
            categoriasSelecionadas.push(estabelecimentoCategoriaId);
        }
    });

    // Existem alguns possíveis casos:
    // #01 - Se não tiver filtro (categoriasSelecionadas) ou a opções "mostrar todos" estiver ativa;
    // #01.1 - Caso o usuário NÃO tenha escrito nada no #txtFiltro, mostre tudo;
    // #01.2 - Caso o usuário tenha escrito algo no #txtFiltro, filtre com base nisso;
    // #02 - Se tiver filtro (categoriasSelecionadas), filtre com base nos selecionados + escrito pelo usuário no #txtFiltro;

    // Caso #01.1 e #01.2;
    var isMostrarTodos = $('#opcaoMostrarTodasCategorias').attr('data-is-selecionado');
    if (categoriasSelecionadas.length === 0 || isMostrarTodos === '1') {
        if (!busca) {
            // Caso #01.1;
            $('.image-wrapper').filter(function () {
                $(this).fadeIn('slow');
            });
        } else {
            // Caso #01.2;
            $('.image-wrapper').filter(function () {
                var data;
                var tipo = $(this).attr('data-tipo');
                var descricao = $(this).attr('data-desc');

                // Se tiver tipo, significa que é o filtro inicial (data-tipo = tipo de estabelecimento);
                if (tipo) {
                    data = tipo + ' ' + descricao;
                } else {
                    // Else, ou seja, se não tiver tipo significa que é outro filtro (data-nome = nome do estabelecimento);
                    var nome = $(this).attr('data-nome');
                    data = nome + ' ' + descricao;
                }

                // O data foi definido no if acima, use-o para filtrar;
                if (data.toLowerCase().indexOf(busca) > -1) {
                    $(this).fadeIn('slow');
                } else {
                    $(this).fadeOut('slow');
                }
            });
        }
    } else {
        // Caso #02;
        $('.image-wrapper').each(function () {
            var categoriaId = $(this).attr('data-categoria-id');
            var tipo = $(this).attr('data-tipo');
            var isExiste = $.inArray(categoriaId, categoriasSelecionadas);

            // Se a categoria que está no each atual estiver na lista categoriasSelecionadas...
            if (isExiste > -1) {
                // Além da verificação acima, verifique também o que foi escrito no #txtFiltro;
                if (tipo.toLowerCase().indexOf(busca) > -1) {
                    $(this).fadeIn('slow');
                } else {
                    $(this).fadeOut('slow');
                }
            } else {
                // Se a categoria que está no each atual NÃO estiver na lista categoriasSelecionadas, esconda-a;
                $(this).fadeOut('slow');
            }
        });
    }

    // Verificar a quantidade itens que foram encontrados para exibir mensagem ou não;
    setTimeout(function () {
        MensagemTiposEstabelecimentos();
    }, 700);
}

function MensagemTiposEstabelecimentos() {
    $('#imgMensagemTiposEstabelecimentos').remove();
    $('#spanMensagemTiposEstabelecimentos').remove();
    var qtdExibidos = $('.image-wrapper:visible').length;
    // alert(qtdExibidos);

    if (qtdExibidos < 1) {
        var imagem = `
            <div class="mt-4" id="imgMensagemTiposEstabelecimentos">
                <figure class="image is-256x256 has-image-centered sem-highlight">
                    <img src="/static/svg/triste-1.svg">
                </figure>
            </div>
        `;

        var msg = `
            <div class="has-text-centered" id="spanMensagemTiposEstabelecimentos">
                <span>
                    Nenhum registro foi encontrado com o filtro utilizado 
                </span>
            </div>

            ` + imagem + `
        `;

        $('.section-part').append(msg);
    }
}
