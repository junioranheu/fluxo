$(document).ready(function () {
    var numeroEndereco = $('#txtNumeroEndereco').val();
    var rua = $('#txtRua').val();
    var cidade = $('#txtCidade').val();
    var nomeEstabelecimento = $('#txtNomeEstabelecimento').val();

    if (!rua || !cidade) {
        Aviso('warning', 'Houve um erro ao encontrar a localização desse estabelecimento no mapa! ' + Emoji('triste'), true, 4000);
        console.log('Rua: ' + rua + '. Cidade: ' + cidade);
        return false;
    }

    var baseUrlNomatim = 'https://nominatim.openstreetmap.org/';
    var urlPais = 'country=Brazil';
    var urlCidade = 'city=' + cidade;
    var urlRua = (numeroEndereco) ? ('street=' + numeroEndereco + ' ' + rua) : ('street=' + rua);
    var urlFormato = 'format=json';
    var urlParametrosNomatim = 'search?' + urlPais + '&' + urlCidade + '&' + urlRua + '&' + urlFormato;
    var urlFinalNominatim = baseUrlNomatim + urlParametrosNomatim;
    // alert(urlFinalNominatim);

    $.ajax({
        url: urlFinalNominatim,
        type: 'GET',
        dataType: 'json', // added data type
        success: function (res) {
            // console.log(res);

            if (res[0]) {
                var latitude = res[0].lat;
                var longitude = res[0].lon;
                DefinirMapa(latitude, longitude, nomeEstabelecimento);
            } else {
                var msg = 'A rua "' + rua.replace("Rua ", "") + '" da cidade "' + cidade + '" não foi localizada no mapa!';
               // Aviso('warning', msg + ' ' + Emoji('triste'), true, 4000);
                ErroEncontrarMapa(msg);
            }
        },
        error: function (request, status, error) {
            console.log(request.responseText);
            var msg = 'Houve um erro ao encontrar a localização desse estabelecimento no mapa!';
            Aviso('error', msg + ' ' + Emoji('triste'), true, 4000);
            ErroEncontrarMapa(msg);
        }
    });
});

function DefinirMapa(lat, long, nomeEstabelecimento) {
    // alert('lat: ' + lat + ' | long: ' + long);
    var map = L.map('mapa').setView([lat, long], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    L.marker([lat, long]).addTo(map)
        .bindPopup(nomeEstabelecimento)
        .openPopup();
}

function ErroEncontrarMapa(msg) {
    // $('#mapa').remove();
    // $('#divMapa').append(msg);

    // Apagar div;
    $('#divMapa').remove();
}