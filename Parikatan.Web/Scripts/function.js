function SubmitComment() {


    var url = window.location.pathname;
    var id = url.substring(url.lastIndexOf('/') + 1);

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var bodyVal = $("#commentBody").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/ProductComments/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, code: id },
                type: "GET"
            }).done(function (result) {
                if (result === "true") {
                    $("#errorDiv").css('display', 'none');
                    $("#SuccessDiv").css('display', 'block');
                    localStorage.setItem("id", "");
                }
                else if (result === "InvalidEmail") {
                    $("#errorDiv").html('ایمیل وارد شده صحیح نمی باشد.');
                    $("#errorDiv").css('display', 'block');
                    $("#SuccessDiv").css('display', 'none');

                }
            });
    }
    else {
        $("#errorDiv").html('تمامی فیلد های زیر را تکمیل نمایید.');
        $("#errorDiv").css('display', 'block');
        $("#SuccessDiv").css('display', 'none');

    }
}


function addDiscountCode() {
    var coupon = $("#coupon").val();
    $('#errorDiv-discount').css('display', 'none');
    if (coupon !== "") {
        $.ajax(
            {
                url: "/Basket/DiscountRequestPost",
                data: { coupon: coupon },
                type: "GET"
            }).done(function (result) {
                if (result !== "Invald" && result !== "Used" && result !== "Expired" && result !== "maxtotal" && result !== "notFirstOrder") {
                    location.reload();
                }
                else if (result !== true) {
                    $('#errorDiv-discount').css('display', 'block');
                    if (result.toLowerCase() === "used") {
                        $('#errorDiv-discount').html("این کد تخفیف قبلا استفاده شده است.");
                    }
                    else if (result.toLowerCase() === "expired") {
                        $('#errorDiv-discount').html("کد تخفیف شما منقضی شده است..");
                    }
                    else if (result.toLowerCase() === "notfirstorder") {
                        $('#errorDiv-discount').html("این کد مخصوص خرید اول از سایت نیکلاس می باشد.");
                    }
                    else if (result.toLowerCase() === "maxtotal") {
                        $('#errorDiv-discount').html("حداقل خرید برای استفاده از کد تخفیف 300،000 تومان می باشد.");
                    }
                    else if (result.toLowerCase() === "invald") {
                        $('#errorDiv-discount').html("کد تخفیف نامعتبر است.");
                    }
                    else if (result.toLowerCase() === "true") {
                        $('#SuccessDiv-discount').css('display', 'block');
                        $('#errorDiv-discount').css('display', 'none');
                    }
                }
            });

    } else {
        $('#SuccessDiv-discount').css('display', 'none');
        $('#errorDiv-discount').html('کد تخفیف را وارد نمایید.');
        $('#errorDiv-discount').css('display', 'block');
    }
}

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function addToBasket(code, qty) {
    if (qty === 'detail') {
        qty = $('#basketQty').val();
    }

    $.ajax(
        {
            url: "/cart",
            data: { code: code, qty: qty },
            type: "Post"
        }).done(function (result) {
            if (result !== true) {
                window.location = "/basket";
            }
        });
}

function DisappearButton() {
    $('#btnPayment').css('display', 'none');
    $('#redirect-to-bank').css('display', 'block');
}

function AppearButton() {
    $('#btnPayment').css('display', 'block');
    $('#redirect-to-bank').css('display', 'none');

}

function postToken(token) {
    var form = document.createElement("form");
    form.setAttribute("method", "Post");
    form.setAttribute("action", "https://pay.apsan.co/v1/payment");
    form.setAttribute("target", "_self");
    var hiddenField = document.createElement("input");
    hiddenField.setAttribute("name", "Token");
    hiddenField.setAttribute("value", token);
    form.appendChild(hiddenField);
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}
function finalizeOrder() {
    DisappearButton();
    var orderNotes = $('#order_note').val();
    var cellnumber = $('#cellnumber').val();
    var postalCode = $('#postalCode').val();
    var address = $('#address').val();
    var city = $('#ddlcity option:selected').val();
    var fullname = $('#fullname').val();

    //  var gateway = $('input[name="payment_method"]:checked').val();


    if (city === undefined || city === '0' || city === 0)
        city = "";




    if (fullname !== "" && cellnumber !== "" && address !== "" && city !== "") {
        $.ajax(
            {
                url: "/Basket/Finalize",
                data: {
                    notes: orderNotes, cellnumber: cellnumber,
                    postal: postalCode, address: address,
                    city: city, fullname: fullname,
                    bank: "zarinpal"
                },
                type: "GET"
            }).done(function (result) {
                if (result.includes('nonstock')) {
                    var products = result.split('|');
                    var productNames = "";
                    for (var i = 1; i < products.length - 1; i++) {
                        productNames = productNames + "\"" + products[i] + "\"";
                    }

                    $('#error-box').css('display', 'block');
                    $('#error-box').html('کاربر گرامی موجودی کالای ' +
                        productNames +
                        'از موجودی کالا انتخابی شما کمتر می باشد. لطفا به ' +
                        '<a href="/basket">سبد خرید </a>' +
                        ' خود مراجعه کنید و موجودی را تغییر دهید.');
                    AppearButton();

                }

                //else if (result.includes('notonline')) {

                //    var orderId = result.split('|')[1];
                //    window.location = "/thank-you?orderid=" + orderId;
                //    //$('#error-box').css('display', 'none');
                //    //$('#success-checkout-box').css('display', 'block');
                //    //$('#success-checkout-box').html('با تشکر از خرید شما. همکاران ما جهت هماهنگی ارسال با شما تماس خواهند گرفت');
                //    //AppearButton();

                //}
                else if (result === 'emptyBasket') {

                    window.location = "/checkout";

                }
                else if (result !== "false") {
                    //if (gateway === "pec" || gateway === "zarinpal" || gateway === "snapp") {
                    window.location = result;
                    //} else if (gateway === "tally") {
                    //    var token = result.split('true*')[1];

                    //    postToken(token);
                    //}
                } else {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                    $('#success-checkout-box').css('display', 'none');

                    AppearButton();

                }
            });
    }
    else {
        $('#error-box').css('display', 'block');
        $('#error-box').html('تمامی فیلدهای ستاره دار باید تکمیل شود');
        $('#success-checkout-box').css('display', 'none');
        AppearButton();
    }


}


function finalizeOrderQuickBasket() {
    DisappearButton();
    var cellnumber = $('#cellnumber').val();
    var fullname = $('#fullname').val();

    //  var gateway = $('input[name="payment_method"]:checked').val();







    if (fullname !== "" && cellnumber !== "") {
        $.ajax(
            {
                url: "/Basket/Finalize",
                data: {
                    notes: "quickBasket",
                    cellnumber: cellnumber,
                    fullname: fullname,
                    bank: "zarinpal"
                },
                type: "GET"
            }).done(function (result) {
                if (result.includes('nonstock')) {
                    var products = result.split('|');
                    var productNames = "";
                    for (var i = 1; i < products.length - 1; i++) {
                        productNames = productNames + "\"" + products[i] + "\"";
                    }

                    $('#error-box').css('display', 'block');
                    $('#error-box').html('کاربر گرامی موجودی کالای ' +
                        productNames +
                        'از موجودی کالا انتخابی شما کمتر می باشد. لطفا به ' +
                        '<a href="/basket">سبد خرید </a>' +
                        ' خود مراجعه کنید و موجودی را تغییر دهید.');
                    AppearButton();

                }

                //else if (result.includes('notonline')) {

                //    var orderId = result.split('|')[1];
                //    window.location = "/thank-you?orderid=" + orderId;
                //    //$('#error-box').css('display', 'none');
                //    //$('#success-checkout-box').css('display', 'block');
                //    //$('#success-checkout-box').html('با تشکر از خرید شما. همکاران ما جهت هماهنگی ارسال با شما تماس خواهند گرفت');
                //    //AppearButton();

                //}
                else if (result === 'emptyBasket') {

                    window.location = "/checkout";

                }
                else if (result !== "false") {
                    //if (gateway === "pec" || gateway === "zarinpal" || gateway === "snapp") {
                    window.location = result;
                    //} else if (gateway === "tally") {
                    //    var token = result.split('true*')[1];

                    //    postToken(token);
                    //}
                } else {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                    $('#success-checkout-box').css('display', 'none');

                    AppearButton();

                }
            });
    }
    else {
        $('#error-box').css('display', 'block');
        $('#error-box').html('تمامی فیلدهای ستاره دار باید تکمیل شود');
        $('#success-checkout-box').css('display', 'none');
        AppearButton();
    }


}


function finalizeOrderInCallBack(id) {
    DisappearButton();
    var cellnumber = $('#cellnumber').val();
    var postalCode = $('#postalCode').val();
    var address = $('#address').val();
    var city = $('#ddlcity').val();
    var fullname = $('#fullname').val();



    if (fullname !== "" && cellnumber !== "" && address !== "" && city !== "") {
        $.ajax(
            {
                url: "/Basket/FinalizeInCallBack",
                data: {

                    cellnumber: cellnumber,
                    postal: postalCode, address: address,
                    city: city,
                    fullname: fullname,
                    bank: "zarinpal",
                    orderId: id
                },
                type: "GET"
            }).done(function (result) {
                if (result === "true") {
                    $('#success-checkout-box').css('display', 'block');
                    $('#success-checkout-box').html('اطلاعات شما با موفقیت ثبت گردید');
                } else {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                    $('#success-checkout-box').css('display', 'none');

                    AppearButton();

                }
            });
    }
    else {
        $('#error-box').css('display', 'block');
        $('#error-box').html('تمامی فیلدهای ستاره دار باید تکمیل شود');
        $('#success-checkout-box').css('display', 'none');
        AppearButton();
    }


}

function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

//function updateBasket() {
//    $('.loading-fuulpage').css('display', 'block');
//    var cookie = getCookie('basket-nicolas');

//    var orderDetails = cookie.split('/');
//    var newOrderDetails = '';

//    for (var i = 0; i < orderDetails.length - 1; i++) {
//        var orderDetail = orderDetails[i].split('^');

//        var txtQtyVal = $('#txt-qty-' + orderDetail[0]).val();

//        var qty = parseInt(txtQtyVal);
//        if (qty > 0) {
//            if (txtQtyVal !== orderDetail[1]) {
//                orderDetails[i] = orderDetail[0] + '^' + txtQtyVal;
//            }
//            newOrderDetails = newOrderDetails + orderDetails[i] + '/';
//        }
//    }

//    deleteCookie('basket-nicolas');
//    deleteCookie('discount');
//    setCookie('basket-nicolas', newOrderDetails, 100);
//    location.reload();
//}

function search() {
    var searchTerm = $('#TextSearch').val();

    //window.location = "/product?searchterm=" + searchTerm;

    //window.location.replace("/product?searchterm=" + searchTerm);
    //return false;


    window.onload = function () {
        document.getElementById("search-form").onsubmit = function () {
            window.location.replace("/product?searchterm=" + searchTerm);
            return false;
        }
    }
}

function postNewsletter() {
    var newsletter = $('#txtNewsletter').val();
    if (newsletter === '') {
        $('#danger-alert-nl').css('display', 'block');
        $('#success-alert-nl').css('display', 'none');
        $('#danger-alert-nl').html('ایمیل خود را وارد نمایید');
    } else {
        $.ajax(
            {
                url: "/Newsletters/EmailPost",
                data: { email: newsletter },
                type: "Post"
            }).done(function (result) {
                if (result === "true") {
                    $('#danger-alert-nl').css('display', 'none');
                    $('#success-alert-nl').css('display', 'block');
                } else if (result == "invalidemail") {
                    $('#danger-alert-nl').css('display', 'block');
                    $('#success-alert-nl').css('display', 'none');
                    $('#danger-alert-nl').html('ایمیل وارد شده صحیح نمی باشد');
                } else {
                    $('#danger-alert-nl').css('display', 'block');
                    $('#success-alert-nl').css('display', 'none');
                    $('#danger-alert-nl').html('خطایی رخ داده است. لطفا دقایقی دیگر مجدادا تلاش کنید');
                }
            });
    }
}


function SubmitBlogComment(id) {

    //var url = window.location.pathname;
    //var id = url.substring(url.lastIndexOf('/') + 1);

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var bodyVal = $("#commentBody").val();
    var siteVal = $("#commentSite").val();
    if (nameVal !== "" && emailVal !== "" && bodyVal !== "") {
        $.ajax(
            {
                url: "/BlogComments/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, code: id, site: siteVal },
                type: "POST"
            }).done(function (result) {
                if (result === "true") {
                    $("#errorDivBlog").css('display', 'none');
                    $("#SuccessDivBlog").css('display', 'block');
                    localStorage.setItem("id", "");
                }
                else if (result === "InvalidEmail") {
                    $("#errorDivBlog").html('ایمیل وارد شده صحیح نمی باشد.');
                    $("#errorDivBlog").css('display', 'block');
                    $("#SuccessDivBlog").css('display', 'none');

                }
            });
    }
    else {
        $("#errorDivBlog").html('تمامی فیلد های زیر را تکمیل نمایید.');
        $("#errorDivBlog").css('display', 'block');
        $("#SuccessDivBlog").css('display', 'none');

    }
}

function SubmitContactUs() {

    var nameVal = $("#commentName").val();
    var emailVal = $("#commentEmail").val();
    var phoneVal = $("#phone").val();
    var bodyVal = $("#commentBody").val();
    if (nameVal !== "" && bodyVal !== "" && phoneVal !== "") {
        $.ajax(
            {
                url: "/ContactUsForms/SubmitComment",
                data: { name: nameVal, email: emailVal, body: bodyVal, phone: phoneVal },
                type: "POST"
            }).done(function (result) {
                if (result === "true") {
                    $("#errorDivContact").css('display', 'none');
                    $("#SuccessDivContact").css('display', 'block');
                    localStorage.setItem("id", "");
                }
                else if (result === "InvalidEmail") {
                    $("#errorDivContact").html('ایمیل وارد شده صحیح نمی باشد.');
                    $("#errorDivContact").css('display', 'block');
                    $("#SuccessDivContact").css('display', 'none');

                }
            });
    }
    else {
        $("#errorDivContact").html('تمامی فیلد های بالا را تکمیل نمایید.');
        $("#errorDivContact").css('display', 'block');
        $("#SuccessDivContact").css('display', 'none');

    }
}


function searchResult() {
    var searchQuery = $('#search_input').val();
    location.href = "/result?searchquery=" + searchQuery;
}
function submitSearchResult() {
    var searchQuery = $('#txtsearch').val();
    location.href = "/result?searchquery=" + decodeURIComponent(searchQuery);
}
function submitSearchForSearchPageResult() {
    var searchQuery = $('#txtsearchpage').val();
    location.href = "/result?searchquery=" + decodeURIComponent(searchQuery);
}


function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}


function getItem(code, title, amount, imageUrl, stock, discountAmount) {
    return "		<div class='col-md-3 col-6 newclass'><div class='product'><div>" +
        "<a href='/product/" + code + "'><img src='" + imageUrl + "' alt='" + title + "'></a></div>" +
        getNoStockTitle(stock) +
        "<div class='product_info'><h6 class='product_title'><a href='/product/" + code + "'>" + title + "</a></h6>" +
        "<div class='product_price'>" + getAmount(amount, discountAmount) + " </div>" +
        getAddToBasketButton(code, stock) +
        "</div></div></div>";
}
function getAmount(amount, discountAmout) {
    if (discountAmout === '') {
        return "<span class='price'>" + amount + "</span><del></del>";
    } else {
        return "<span class='price'>" + discountAmout + "</span><del>" + amount + "</del>";
    }

}
function getAddToBasketButton(code, stock) {
    var stockInt = parseInt(stock);

    if (stockInt > 0) {
        //return "<div class='add-to-cart'><button class='btn btn-fill-out btn-addtocart' onclick='addToBasket('" +
        //    code +
        //    "', '1');'><i class='icon-basket-loaded'></i>خرید</button></div>";
        return "<div class='add-to-cart'><a class='btn btn-fill-out btn-addtocart' href='/basketRedirect?code=" + code + "&qty=1'><i class='icon-basket-loaded'></i>خرید</a></div>";
    } else {
        return "<div class='add-to-cart'><button class='btn btn-addtocart btn-disable' disabled='disabled' onclick='addToBasket('" +
            code +
            "', '1');'><i class='icon-basket-loaded'></i>خرید</button></div>";
    }
}
function getNoStockTitle(stock) {
    var stockInt = parseInt(stock);

    if (stockInt === 0) {
        return " <span class='pr_flash bg-danger'>ناموجود</span>";
    } else {
        return "";
    }
}


$(".show-more a").on("click", function () {
    var $this = $(this);
    var $content = $this.parent().prev("div.content");
    var linkText = $this.text().toUpperCase();

    //if (linkText === "SHOW MORE") {
    //	linkText = "Show less";

    //	$content.switchClass("hideContent", "showContent", 400);
    $('.content').removeClass('hideContent');
    $('.content').addClass('showContent');
    $('.show-more-link').css('display', 'none');
    //} else {
    //	linkText = "Show more";
    //	$content.switchClass("showContent", "hideContent", 400);
    //};

    $this.text(linkText);
});




