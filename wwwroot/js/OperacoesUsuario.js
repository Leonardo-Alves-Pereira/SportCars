function AprovarUsuario(usuarioId, nome) {
    const url = "/Usuarios/AprovarUsuario";

    $.ajax({
        method: 'POST',
        url: url,
        data: { usuarioId: usuarioId },
        success: function (data) {
            if (data == true) {
                var classes = $("#" + usuarioId).attr('class');
                console.log(classes);
                $("#" + usuarioId).removeClass(classes).addClass('font-weight-bold bg-success text-white p-2 rounded').text('Aprovado');
                $("." + usuarioId).attr('style', 'display: block;')
                $("." + usuarioId).append(' <a href="Usuarios/GerenciarUsuarios?usuarioId=' + usuarioId + '&nome=' + nome + '" class= "text-primary" style="font-size:20pt;" asp-controller="Usuarios" asp-action="GerenciarUsuarios" asp-route-usuarioId="' + usuarioId + '" aps-route-nome="' + nome + '"><i class="fa fa-group"></i></a>')
                alertify.success('Usuario aprovado com sucesso.');
            } else {
                alertify.error('Erro ao aprovar usuario.');
            }
        }
    });
}

function ReprovarUsuario(usuarioId) {
    const url = "/Usuarios/ReprovarUsuario";

    $.ajax({
        method: 'POST',
        url: url,
        data: { usuarioId: usuarioId },
        success: function (data) {
            if (data == true) {
                var classes = $("#" + usuarioId).attr('class');
                $("#" + usuarioId).removeClass(classes).addClass('font-weight-bold bg-danger text-white p-2 rounded').text('Reprovado');
                $("." + usuarioId).attr('style', 'display: none;')
                $("." + usuarioId + " a").remove();
                alertify.error('Usuario reprovado com sucesso.');
            } else {
                alertify.error('Error ao reprovar usuario.');
            }
        }

    });
}