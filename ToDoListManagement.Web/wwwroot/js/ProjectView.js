var startDateValidation = "";
var endDateValidation = "";

$(document).ready(function () {
    $("#addProjectModal").on('hidden.bs.modal', function () {
        $("#addProjectForm").trigger("reset");
        $("#addProjectForm").find(".text-danger").text("");
    });

    $("#updateProjectModal").on('hidden.bs.modal', function () {
        $("#editProjectForm").trigger("reset");
        $("#editProjectForm").find(".text-danger").text("");
    });

    let columns = [
        {
            name: "projectId",
            data: "projectId",
            title: "#",
            orderable: true,
            searchable: false,
            type: 'num',
        },
        {
            name: "projectName",
            data: "projectName",
            title: "Project Name",
            orderable: true,
            searchable: true,
            type: 'string',
        },
        {
            name: "Program Manager",
            data: "pmName",
            title: "Program Manager",
            orderable: false,
            searchable: false,
            type: 'string',
            render: function (data) {
                if (data && data.length > 0) {
                    const randomColor = "#" + Math.floor(Math.random() * 16777215).toString(16).padStart(6, '0');
                    const initial = data.charAt(0).toUpperCase();

                    return `
                        <li class="list-inline-item rounded-circle"
                            style="width: 30px; height: 30px; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; background-color: ${randomColor}; cursor: pointer;"
                            title="${data}">
                            ${initial}
                        </li>
                    `;
                } else {
                    return '<span class="text-secondary">Not Assigned</span>';
                }
            }
        },
        {
            name: "Team Members",
            data: "users",
            title: "Team Members",
            orderable: false,
            searchable: false,
            type: 'string',
            render: function (data) {
                if (!data || data.length === 0) {
                    return '<span class="text-secondary">Not assigned</span>';
                }

                let html = '<ul class="list-inline d-flex m-0">';
                data.forEach(function (user) {
                    const initial = user.name ? user.name.charAt(0).toUpperCase() : '?';
                    const color = "#" + Math.floor(Math.random() * 16777215).toString(16).padStart(6, '0');
                    html += `
                        <li class="list-inline-item rounded-circle"
                            style="width: 30px; height: 30px; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; background-color: ${color}; cursor: pointer;"
                            title="${user.name}">
                            ${initial}
                        </li>`;
                });
                html += '</ul>';
                return html;
            }
        },
        {
            name: "Status",
            data: "status",
            title: "Status",
            orderable: false,
            searchable: false,
            type: 'string',
            render: function (data) {
                switch (data) {
                    case "Active":
                        return '<span class="badge badge-success">Active</span>';
                    case "Completed":
                        return '<span class="badge badge-primary">Completed</span>';
                    case "Cancelled":
                        return '<span class="badge badge-danger">Cancelled</span>';
                    default:
                        return '<span class="badge badge-secondary text-dark">Unknown</span>';
                }
            }
        }
    ];

    if (window.ProjectPermissions.canAddEdit || window.ProjectPermissions.canDelete) {
        columns.push({
            name: "Actions",
            data: null,
            title: "Actions",
            orderable: false,
            searchable: false,
            render: function (row) {
                let html = '';

                if (window.ProjectPermissions.canAddEdit) {
                    html += `
                        <button class="btn btn-info btn-sm text-white" onclick="openEditProjectModal(${row.projectId})">
                            <i class="fas fa-pencil-alt"></i> Edit
                        </button>`;
                }

                if (window.ProjectPermissions.canDelete) {
                    html += `
                        <button class="btn btn-danger btn-sm" onclick="openDeleteProjectModal('${row.projectId}')">
                            <i class="fas fa-trash"></i> Delete
                        </button>`;
                }

                if (window.ProjectPermissions.canAddEdit) {
                    html += `
                        <button class="btn btn-primary btn-sm assign-btn" data-project-id="${row.projectId}">
                            <i class="fa-solid fa-user-plus" style="font-size:12px !important"></i> Assign
                        </button>`;
                }

                return html || '<span class="text-muted">No Actions</span>';
            }
        });
    }

    $("#projectTable").DataTable({
        "serverSide": true,
        "processing": true,
        "ajax": function (data, callback, settings) {
            // Prepare data for the AJAX request
            const ajaxData = {
                draw: data.draw,
                start: data.start,
                length: data.length || 10,
                searchValue: data.search ? data.search.value : "",
                sortColumn: data.order && data.columns ? data.columns[data.order[0].column].data : "",
                sortDirection: data.order ? data.order[0].dir : "asc"
            };

            ajaxCall('/Project/GetProjects', 'GET', JSON.stringify(ajaxData), function (response) {
                callback({
                    draw: response.draw,
                    recordsTotal: response.recordsTotal,
                    recordsFiltered: response.recordsFiltered,
                    data: response.data
                });
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
            },
        },
        "lengthChange": true,
        "searching": true,
        "scrollCollapse": true,
        "scrollY": '455px'
    });
});

function openAddProjectModal() {
    ajaxCall('/Project/AddProject', 'GET', null, function (response) {
        if (response.success === false) {
            toastr.error(response.message);
        }
        else {
            $("#addProjectModal").html(response);
            $("#addProjectModal").modal('show');
            $.validator.unobtrusive.parse("#addProjectForm");
        }
    });
}

function openDeleteProjectModal(projectId) {
    $("#deleteProjectLink").attr("href", `/Project/DeleteProject?projectId=${projectId}`);
    $('#deleteProjectModal').modal('show');
}

function openEditProjectModal(projectId) {
    ajaxCall(`/Project/GetProjectById?projectId=${projectId}`, 'GET', null, function (response) {
        if (response.success === false) {
            toastr.error(response.message);
        }
        else {
            $("#updateProjectModal").html(response);
            $("#updateProjectModal").modal('show');
            $.validator.unobtrusive.parse("#editProjectForm");
            const startDateInput = $('#editProjectStartDate');

            startDateValidation = startDateInput.attr('min');
        }
    });
}

var selectedProjectId = null;
$(document).on('click', '.assign-btn', function () {
    selectedProjectId = $(this).data('project-id');
    selectedUserIds.clear();

    $('#assignMembersModal').modal('show').on('shown.bs.modal', function () {
        const table = $('#assignTable').DataTable();
        table.columns.adjust().draw();
    });
});

if (window.ProjectPermissions.canAddEdit) {
    $("#assignTable").DataTable({
        "serverSide": true,
        "processing": true,
        "ajax": {
            "url": "/Project/GetAssignedMembers",
            "type": "GET",
            "datatype": "json",
            "data": function (d) {
                return {
                    projectId: selectedProjectId,
                    draw: d.draw,
                    start: d.start,
                    length: d.length || 10,
                    searchValue: d.search ? d.search.value : "",
                    sortColumn: d.order && d.columns ? d.columns[d.order[0].column].data : "",
                    sortDirection: d.order ? d.order[0].dir : "asc"
                };
            },
            "dataSrc": function (json) {
                json.data.forEach(item => {
                    if (item.isAssigned) {
                        selectedUserIds.add(item.userId);
                    }
                });
                return json.data;
            }
        },
        "search": {
            caseInsensitive: true
        },
        "columns": [
            {
                name: "mainCheckBox",
                data: "userId",
                title: `<input type="checkbox" id="mainCheckBoxHeader" class="form-check-input">`,
                orderable: false,
                searchable: false,
                className: "no-sort",
                type: 'input',
                render: function (data, type, row) {
                    return `<input type="checkbox" class="form-check-input row-checkbox" data-id="${data}"/>`;
                }
            },
            {
                name: "Name",
                data: "name",
                title: "Name",
                orderable: true,
                searchable: true,
                type: 'string',
            },
            {
                name: "Email",
                data: "email",
                title: "Email",
                orderable: true,
                searchable: true,
                type: 'string',
            },
            {
                name: "Phone Number",
                data: "phoneNumber",
                title: "Phone Number",
                orderable: true,
                searchable: true,
                type: 'string',
            }
        ],
        "lengthMenu": [5, 10, 15, 20],
        "paging": true,
        "language": {
            "paginate": {
                "previous": "&laquo;",
                "next": "&raquo;"
            },
        },
        "lengthChange": true,
        "searching": true,
        "scrollCollapse": true,
        "scrollY": '455px',
        "drawCallback": function () {
            $('#assignTable input.row-checkbox').each(function () {
                const id = Number($(this).data('id'));
                $(this).prop('checked', selectedUserIds.has(id));
            });

            const total = $('#assignTable input.row-checkbox').length;
            const checked = $('#assignTable input.row-checkbox:checked').length;
            $('#mainCheckBoxHeader').prop('checked', total > 0 && total === checked);
        }
    });
}

$(document).on('change', '.row-checkbox', function () {
    const userId = parseInt($(this).data('id'));
    if ($(this).is(':checked')) {
        selectedUserIds.add(userId);
    } else {
        selectedUserIds.delete(userId);
    }

    const total = $('#assignTable input.row-checkbox').length;
    const checked = $('#assignTable input.row-checkbox:checked').length;
    $('#mainCheckBoxHeader').prop('checked', total > 0 && total === checked);
});

$('#mainCheckBoxHeader').on('change', function () {
    const isChecked = $(this).is(':checked');
    $('#assignTable input.row-checkbox').each(function () {
        const userId = parseInt($(this).data('id'));
        $(this).prop('checked', isChecked);
        if (isChecked) {
            selectedUserIds.add(userId);
        } else {
            selectedUserIds.delete(userId);
        }
    });
});

let selectedUserIds = new Set();

$('#assignMembersForm').on('submit', function (e) {
    e.preventDefault();

    const payload = {
        ProjectId: selectedProjectId,
        UserIds: Array.from(selectedUserIds)
    };

    $.ajax({
        url: '/Project/AssignMembers',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (response) {
            $('#assignMembersModal').modal('hide');
            toastr.success(response.message);
            $('#projectTable').DataTable().ajax.reload();
        },
        error: function () {
            toastr.error(response.message);
        }
    });
});

const today = new Date().toISOString().split('T')[0];

$(document).on('change', '#addProjectStartDate', function () {
    const fromDate = $(this).val();
    if (fromDate) {
        $("#addProjectDueDate").attr("min", fromDate);
    } else {
        $("#addProjectDueDate").attr("min", today);
    }
});

$(document).on('change', '#addProjectDueDate', function () {
    const toDate = $(this).val();
    if (toDate) {
        $("#addProjectStartDate").attr("max", toDate);
    } else {
        $("#addProjectStartDate").removeAttr("max");
    }
});

$(document).on('change', '#editProjectStartDate', function () {
    const fromDate = $(this).val();
    if (fromDate) {
        $("#editProjectDueDate").attr("min", fromDate);
    } else {
        $("#editProjectDueDate").attr("min", startDateValidation);
    }
});

$(document).on('change', '#editProjectDueDate', function () {
    const toDate = $(this).val();
    if (toDate) {
        $("#editProjectStartDate").attr("max", toDate);
    } else {
        $("#editProjectStartDate").removeAttr("max");
    }
});