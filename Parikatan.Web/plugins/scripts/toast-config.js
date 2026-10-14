function showError(message) {
  toastr.error(message, "خطا", {
    timeOut: 2000,
    positionClass: "toast-top-full-width ",
    progressBar: true,
    showMethod: "fadeIn",
    hideMethod: "fadeOut",
  });
}

function showSuccess(message) {
  toastr.success(message, "تبریک", {
    timeOut: 2000,
    positionClass: "toast-top-full-width ",
    progressBar: true,
    showMethod: "fadeIn",
    hideMethod: "fadeOut",
  });
}
0;
