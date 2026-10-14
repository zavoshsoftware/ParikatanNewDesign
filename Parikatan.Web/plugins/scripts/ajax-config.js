function ajaxPost(url, data, successFunction, errorFunction) {
  $.ajax({
    url: url,
    data: data,
    type: "POST",
    dataType: "JSON",
    beforeSend: function () {
      startPreLoader();
    },
    success: function (response) {
      successFunction(response);
    },
    error: function (xhr) {
      try {
        var error = JSON.parse(xhr.responseText);
        $utils.errorAlert(error.ExceptionMessage);
      } catch (e) {
        console.log(e);
        $utils.errorAlert(xhr.responseText);
      }
      if (errorFunction) {
        errorFunction();
      }
    },
    complete: function () {
      endPreLoader();
    },
  });
}

function startPreLoader() {
  console.log("start");
}

function endPreLoader() {
  console.log("end");
}
