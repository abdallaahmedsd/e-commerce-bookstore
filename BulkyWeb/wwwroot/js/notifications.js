import '../lib/toastr.js/toastr.min.js';

$(function () {
    var successMessage = $('#toastr-success-message').data('message');
    if (successMessage) {
        toastr.success(successMessage);
    }

    var errorMessage = $('#toastr-error-message').data('message');
    if (errorMessage) {
        toastr.error(errorMessage);
    }

    var warningMessage = $('#toastr-warning-message').data('message');
    if (warningMessage) {
        toastr.warning(warningMessage);
    }

    var infoMessage = $('#toastr-info-message').data('message');
    if (infoMessage) {
        toastr.info(infoMessage);
    }
});
