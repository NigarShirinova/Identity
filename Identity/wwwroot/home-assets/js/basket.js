$(function () {
    $(".addtobasket").on('click', function () {
        $.ajax(
            {
                method: "POST",
                url: "@Url.Action("AddProduct", "Basket")",
                data: {
                    productId: $(this).data('id')
                },
                success: function (response) {
                    alert(response)
                },
                error: function (response) {
                    alert(response)
                }
            }
        )
    })
}


