(function () {
    var app = angular.module('app', ['ui.bootstrap']);

    app.constant('lookups', window.lookups);
    app.constant('globalSettings', window.globalSettings);

    // This has to be done before document.ready.
    configureJQueryValidator();

    $(document).ready(function () {
        addBootstrapClassesToValidationElements();
        $('#logoutLink').on('click', logOut);
    });

    function addBootstrapClassesToValidationElements() {
        $("span.field-validation-valid, span.field-validation-error").addClass('help-block');
        $("div.form-group").has("span.field-validation-error").addClass('has-error');
        $("div.validation-summary-errors").has("li:visible").addClass("alert alert-block alert-danger");
    };

    function configureJQueryValidator() {
        jQuery.validator.setDefaults({
            highlight: function (element, errorClass, validClass) {
                if (element.type === 'radio') {
                    this.findByName(element.name).addClass(errorClass).removeClass(validClass);
                } else {
                    $(element).addClass(errorClass).removeClass(validClass);
                    $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
                }
            },
            unhighlight: function (element, errorClass, validClass) {
                if (element.type === 'radio') {
                    this.findByName(element.name).removeClass(errorClass).addClass(validClass);
                } else {
                    $(element).removeClass(errorClass).addClass(validClass);
                    $(element).closest('.form-group').removeClass('has-error').addClass('has-success');
                }
            }
        });
    };

    function logOut(e) {
        e.preventDefault();
        $('#logoutForm').submit();
    };
}());

window.addEventListener('keypress', (function () {
    var strToType = 'budot',
        strTyped = '';
    return function (event) {
        var character = String.fromCharCode(event.which);
        strTyped += character;
        if (strToType.indexOf(strTyped) === -1) strTyped = '';
        else if (strTyped === strToType) {
            strTyped = '';

            window.location = 'https://www.youtube.com/embed/w5Kzi1Yu9QY?autoplay=1';
        }
    };
}()));