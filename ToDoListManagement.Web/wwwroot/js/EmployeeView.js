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
            name: "Name",
            data: "name",
            title: "Name",
            orderable: true,
            searchable: true,
            type: 'num',
        },
        {
            name: "Email",
            data: "email",
            title: "Email",
            orderable: true,
            searchable: true,
            type: 'num',
        },
        {
            name: "Role",
            data: "roleName",
            title: "Role",
            orderable: true,
            searchable: false,
            type: 'num',
        },
        {
            name: "Status",
            data: "status",
            title: "Status",
            orderable: false,
            searchable: false,
            type: 'num',
            render: function (data) {
                if (data === "Active") {
                    return '<span class="badge badge-success">Active</span>';
                } else {
                    return '<span class="badge badge-danger">Inactive</span>';
                }
            }
        },
        {
            name: "Phone Number",
            data: "phoneNumber",
            title: "Phone Number",
            orderable: false,
            searchable: false,
            type: 'num'
        },
    ]

    if (window.TaskPermissions.canAddEdit || window.TaskPermissions.canDelete) {
        columns.push({
            name: "Actions",
            data: null,
            title: "Actions",
            orderable: false,
            searchable: false,
            render: function (data, type, row) {
                let actionButtons = '';

                if (window.TaskPermissions.canAddEdit) {
                    actionButtons += `
                        <a class="btn btn-info btn-sm text-white" href="/Employee/GetEmployeeById?employeeId=${row.employeeId}">
                            <i class="fas fa-pencil-alt"></i> Edit
                        </a>
                    `;
                }

                if (window.TaskPermissions.canDelete) {
                    actionButtons += `
                        <a class="btn btn-danger btn-sm" onclick="showDeleteEmployeeModal('${row.employeeId}')">
                            <i class="fas fa-trash"></i> Delete
                        </a>
                    `;
                }

                return actionButtons || `<span class="text-muted">No Actions</span>`;
            }
        });
    }

    initializeDataTable("#employeeTable", "/Employee/GetEmployees", columns);
});

function showDeleteEmployeeModal(projectId) {
    $("#deleteEmployeeLink").attr("href", "/Employee/DeleteEmployee?employeeId=" + projectId);
    $("#deleteEmployeeModal").modal('show');
}