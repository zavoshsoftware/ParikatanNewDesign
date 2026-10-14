



function updateCounter(productId, change) {
    var counterElement = document.querySelector('#counter-product-' + productId);
    var currentCount = parseInt(counterElement.innerHTML);

   
    var newCount = currentCount + change;

  
    if (newCount < 1) {
        newCount = 1;
    }

  
    counterElement.innerHTML = newCount;

  
    updateBasket();
}

function updateBasket() {
    $('.loading-fuulpage').css('display', 'block');
    var cookie = getCookie('basket-parikatan');

    var orderDetails = cookie.split('/');
    var newOrderDetails = '';

    for (var i = 0; i < orderDetails.length - 1; i++) {
        var orderDetail = orderDetails[i].split('^');

        var txtQtyVal = $('.txt-qty-' + orderDetail[0]).html();
        var qty = parseInt(txtQtyVal);

        if (qty > 0) {
            if (txtQtyVal !== orderDetail[1]) {
                orderDetails[i] = orderDetail[0] + '^' + txtQtyVal;
            }
            newOrderDetails = newOrderDetails + orderDetails[i] + '/';
        }
    }

    deleteCookie('basket-parikatan');
    deleteCookie('discount');
    setCookie('basket-parikatan', newOrderDetails, 100);
    location.reload();
}
