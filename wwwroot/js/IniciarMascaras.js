$("#CPF").mask("999.999.999-99");
$("#Telefone").mask("(99) 9 9999-9999");

function MostrarSenha(p) {
        event.preventDefault();
        if ($('.' + p + ' input').attr("type") == "text") {
            $('.' + p + ' input').attr('type', 'password');
            $('.' + p + ' div .senha i').addClass("fa-eye-slash");
            $('.' + p + ' div .senha i').removeClass("fa-eye");
        } else if ($('.' + p + ' input').attr("type") == "password") {
            $('.' + p + ' input').attr('type', 'text');
            $('.' + p + ' div .senha i').removeClass("fa-eye-slash");
            $('.' + p + ' div .senha i').addClass("fa-eye");
        }
}

$(document).ready(function () {
    $(".validar").each(function (index) {
        if ($(this).text() <= 0) {
            console.log(index + ": " + $(this).text());
            $(this).attr('style', 'display: none;')
        }
    });
    });