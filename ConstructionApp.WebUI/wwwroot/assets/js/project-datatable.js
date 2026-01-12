$(document).ready(function () {
    $('.project-datatable').DataTable({
        "responsive": true,     // Makes it responsive
        "deferRender": true
        //initComplete: function () {
        //    $('.table-skeleton').hide();
        //    $('.added-table-skeleton').show();
        //}
    });

    $('.task-datatable').DataTable({
        "responsive": true,     // Makes it responsive
        "deferRender": true
        //initComplete: function () {
        //    $('.table-skeleton').hide();
        //    $('.added-table-skeleton').show();
        //}
    });
});

(function () {
    select2 = $('.select2'),
        commentEditor = document.querySelector('.comment-editor'),
        datePicker = document.querySelector('.due-date'),
        startDatePicker = document.querySelector('.start-date');
    // select2
    if (select2.length) {
        function renderLabels(option) {
            if (!option.id) {
                return option.text;
            }
            var $badge = option.text;
            return $badge;
        }

        select2.each(function () {
            var $this = $(this);
            $this.wrap("<div class='position-relative'></div>").select2({
                placeholder: 'Select Label',
                dropdownParent: $this.parent(),
                templateResult: renderLabels,
                templateSelection: renderLabels,
                escapeMarkup: function (es) {
                    return es;
                }
            });
        });
    }
    // datepicker init
    if (datePicker) {
        datePicker.flatpickr({
            monthSelectorType: 'static',
            altInput: true,
            altFormat: 'j F, Y',
            dateFormat: 'Y-m-d'
        });
    }
    // datepicker init
    if (startDatePicker) {
        startDatePicker.flatpickr({
            monthSelectorType: 'static',
            altInput: true,
            altFormat: 'j F, Y',
            dateFormat: 'Y-m-d'
        });
    }

    // FlatPickr Initialization & Validation
    const flatpickrDate = document.querySelector('[name="basicDate"]');

    if (flatpickrDate) {
        flatpickrDate.flatpickr({
            enableTime: false,
            // See https://flatpickr.js.org/formatting/
            dateFormat: 'm/d/Y',
            // After selecting a date, we need to revalidate the field
            onChange: function () {
                fv.revalidateField('basicDate');
            }
        });
    }
})();