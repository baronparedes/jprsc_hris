(function () {
    $(document).ready(function () {
        $('#forget-form').unbind('submit'); // this prevents double submission
        $('#forget-form').on('submit', onForgetFormSubmit);
    });

    function onForgetFormSubmit(e) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();

        var forgetForm = $('#forget-form');

        $.ajax({
            type: 'POST',
            url: forgetForm.attr('action'),
            data: forgetForm.serialize(),
            success: function (data) {
                if (data.status == 'Success') {
                    var returnUrl = $('#ReturnUrl').val();
                    location.replace('/Account/Login?returnUrl=' + encodeURIComponent(returnUrl));
                    //console.log('Success');
                }
                else if (data.status == 'Error') {
                    console.log('Error');
                }
            }
        });

        return false;
    };
}());