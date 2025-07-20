// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function ajaxCall(url, type, data, onSuccess = null, additionalOptions = {}) {
    $.ajax({
        url: url,
        type: type,
        data: JSON.parse(data),
        success: function (response) {
            if (typeof onSuccess === "function") {
                onSuccess(response);
            } else {
                if (response.success) {
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            }
        },
        error: function (xhr, status, error) {
            if (xhr.responseJSON && xhr.responseJSON.message) {
                console.error(xhr.responseJSON.message);
            }
            console.error("An error occurred while processing the request.");
        },
        ...additionalOptions
    });
}

function initializeDataTable(tableId, ajaxUrl, columns, additionalOptions = {}) {
    $(tableId).DataTable({
        "serverSide": true,
        "processing": true,
        "ajax": function (data, callback, settings) {
            const ajaxData = {
                draw: data.draw,
                start: data.start,
                length: data.length || 10,
                searchValue: data.search ? data.search.value : "",
                sortColumn: data.order && data.columns ? data.columns[data.order[0].column].data : "",
                sortDirection: data.order ? data.order[0].dir : "asc",
                statusFilter: $('#statusFilter').val() || "",
                startDateFilter: $('#startDateFilter').val(),
                endDateFilter: $('#endDateFilter').val()
            };

            ajaxCall(ajaxUrl, 'GET', JSON.stringify(ajaxData), function (response) {
                callback({
                    draw: response.draw,
                    recordsTotal: response.recordsTotal,
                    recordsFiltered: response.recordsFiltered,
                    data: response.data
                });
                console.log("Table Data:", response.data);
            });
        },
        "search": {
            caseInsensitive: true
        },
        "columns": columns,
        "lengthMenu": [5, 10, 15, 20],
        "paging": true,
        "language": {
            "paginate": {
                "previous": "&laquo;",
                "next": "&raquo;"
            }
        },
        "lengthChange": true,
        "searching": true,
        "scrollCollapse": true,
        "scrollY": '455px',
        ...additionalOptions
    });
}
