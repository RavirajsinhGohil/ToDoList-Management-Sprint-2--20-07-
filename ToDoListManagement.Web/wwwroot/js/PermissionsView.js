$(document).ready(function () {
    $('.row-checkbox').each(function () {
        const row = $(this).closest('tr');
        const allChecked = row.find('.custom-switch').length === row.find('.custom-switch:checked').length;
        $(this).prop('checked', allChecked);
    });

    updateMainCheckbox();

    $('#selectAll').on('change', function () {
        const isChecked = $(this).is(':checked');
        $('.row-checkbox').prop('checked', isChecked);
        $('.custom-switch').prop('checked', isChecked);
    });

    $('.row-checkbox').on('change', function () {
        const isChecked = $(this).is(':checked');
        const row = $(this).closest('tr');
        row.find('.custom-switch').prop('checked', isChecked);
        updateMainCheckbox();
    });

    $('.custom-switch').on('change', function () {
        const row = $(this).closest('tr');
        const rowCheckbox = row.find('.row-checkbox');

        const canView = row.find('.custom-switch[data-type="CanView"]');
        const canAddEdit = row.find('.custom-switch[data-type="CanAddEdit"]');
        const canDelete = row.find('.custom-switch[data-type="CanDelete"]');

        const type = $(this).data('type');

        if (type === "CanView")
        {
            if (!$(this).is(':checked')) {
                canAddEdit.prop('checked', false);
                canDelete.prop('checked', false);
            }
        } 
        else {
            if ($(this).is(':checked')) {
                canView.prop('checked', true);
            }
        }

        const allChecked = row.find('.custom-switch').length === row.find('.custom-switch:checked').length;
        rowCheckbox.prop('checked', allChecked);

        updateMainCheckbox();
    });

    function updateMainCheckbox() {
        const total = $('.row-checkbox').length;
        const checked = $('.row-checkbox:checked').length;
        $('#selectAll').prop('checked', total > 0 && total === checked);
    }
});