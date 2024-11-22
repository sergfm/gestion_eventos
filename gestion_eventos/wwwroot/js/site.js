function actualizarCarrito() {
    $.ajax({
        url: '/Home/CargarCarritoParcial',
        type: 'GET',
        success: function (data) {
            $('#cart-container').html($(data).find('#cart-container').html());
        },
        error: function () {
            console.error("Error al cargar el carrito.");
        }
    });
}

// Escucha los cambios en los botones de agregar al carrito
$(document).on('click', '.btn-agregar-carrito', function () {
    const eventId = $(this).data('event-id');
    const cantidad = $(this).data('cantidad');

    $.ajax({
        url: '/Home/AgregarAlCarrito',
        type: 'POST',
        data: { eventId, cantidad },
        success: function () {
            actualizarCarrito();
        },
        error: function () {
            console.error("Error al agregar al carrito.");
        }
    });
});
