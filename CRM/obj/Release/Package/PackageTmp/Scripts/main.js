// Simple example, see optional options for more configuration.

        $('#picker3').spectrum({
            type: "color",
            togglePaletteOnly: "true",
            hideAfterPaletteSelect: "true",
            showInput: "true",
            showAlpha: "false",
            showButtons: "false",

            allowEmpty: "false",
            move: function (color) {
                var cambioColor = color.toHexString();


            }
        });


$(document).on('ajaxStart', function () {
    loading_show();
});

$(document).on('ajaxStop', function (start) {
    loading_hide();
});

function loading_show() {
    $('body').loadingModal({
        text: 'Por favor espere',
        animation: 'circle',
    });
    $('body').loadingModal('show');
}

function loading_hide() {
    $('body').loadingModal('hide');
}
