function SubmitForm() {
    disapearButton();
 
    var phoneNumber = $("#phoneNumber").val();
    if (phoneNumber !== "") {
        if (validateIranianMobileNumber(phoneNumber)) {
            $.ajax(
                {
                    url: "/home/SubmitForm",
                    data: { phoneNumber: phoneNumber },
                    type: "POST"
                }).done(function(result) {
                if (result === "true") {
                    $("#errorDiv").css('display', 'none');
                    $("#SuccessDiv").css('display', 'block');
                    apearButton();
                    $("#phone-insert").css('display', 'none');


                } else if (result === "InvalidEmail") {
                    $("#errorDiv").html('شماره موبایل وارد شده صحیح نمی باشد.');
                    $("#errorDiv").css('display', 'block');
                    $("#SuccessDiv").css('display', 'none');
                    apearButton();

                    }
            });
        } else {
            $("#errorDiv").html('شماره موبایل وارد شده صحیح نیست.');
            $("#errorDiv").css('display', 'block');
            $("#SuccessDiv").css('display', 'none');
            apearButton();

        }
    } else {
        $("#errorDiv").html('شماره موبایل خود را وارد نمایید.');
        $("#errorDiv").css('display', 'block');
        $("#SuccessDiv").css('display', 'none');
        apearButton();

    }
}

function validateIranianMobileNumber(mobileNumber) {
    var iranianMobilePattern = /^(?:\+98|0)?9\d{9}$/;
    return iranianMobilePattern.test(mobileNumber);
}

function disapearButton() {
    $('#loading-box').css('display', 'block');
    $('#btnSubmit').css('display', 'none');

}
function  apearButton() {
    $('#loading-box').css('display', 'none');
    $('#btnSubmit').css('display', 'block');

}