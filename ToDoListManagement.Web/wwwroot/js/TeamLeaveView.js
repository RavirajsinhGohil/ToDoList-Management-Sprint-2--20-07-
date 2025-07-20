$(document).ready(function () {
    let columns = [
        {
            name: "Employee Name",
            data: "requestedName",
            title: "Employee Name",
            orderable: true,
            searchable: false,
            type: 'num',
        },
        {
            name: "Start Date",
            data: "startDate",
            title: "Start Date",
            orderable: true,
            searchable: true,
            type: 'num',
        },
        {
            name: "End Date",
            data: "endDate",
            title: "End Date",
            orderable: true,
            searchable: false,
            type: 'num',
        },
        {
            name: "Reason",
            data: "reason",
            title: "Reason",
            orderable: false,
            searchable: false,
            type: 'num',
        },
        {
            name: "Duration",
            data: "duration",
            title: "Duration",
            orderable: false,
            searchable: false,
            type: 'num',
            render: function (data) {
                if (data != null) {
                    return data > 1 ? `${data} day(s)` : `${data} day` ;
                } else {
                    return '';
                }
            }
        },        
        {
            name: "Return Date",
            data: "returnDate",
            title: "Return Date",
            orderable: false,
            searchable: false,
            type: 'num',
        },
        {
            name: "Available On Phone",
            data: "isAvailableOnPhone",
            title: "Available On Phone",
            orderable: false,
            searchable: false,
            type: 'num',
            render: function (data) {
                if (data) {
                    return 'Yes';
                } else {
                    return 'No';
                }
            }
        },
        {
            name: "Approved Date",
            data: "approvedOn",
            title: "Approved Date",
            orderable: false,
            searchable: false,
            type: 'num',
            render: function (data) {
                if (data != null) {
                    return `${data}`;
                } else {
                    return 'Not Approved';
                }
            }
        },
        {
            name: "Status",
            data: "status",
            title: "Status",
            orderable: false,
            searchable: false,
            type: 'num',
            render: function (data) {
                if (data === "Approved") {
                    return '<span class="badge badge-success">Approved</span>';
                } else if (data === "Pending") {
                    return '<span class="badge badge-warning">Pending</span>';
                } else {
                    return '<span class="badge badge-danger">Rejected</span>';
                }
            }
        }        
    ]

    if(window.TeamLeavePermissions.canAddEdit || window.TeamLeavePermissions.canDelete) {
        columns.push({
            name: "Actions",
            data: "returnDate",
            title: "Actions",
            orderable: false,
            searchable: false,
            render: function (data, type, row) {
                let actionButtons = '';
                if (window.TeamLeavePermissions.canAddEdit) {
                    actionButtons += `
                    <button class="btn btn-info btn-sm text-white" onclick="showEditLeaveModal('${row.leaveId}')">
                        <i class="fas fa-pencil-alt"></i> Edit
                    </button>
                    `;
                }
                if (window.TeamLeavePermissions.canDelete) {
                    actionButtons += `
                        <button class="btn btn-danger btn-sm" onclick="showDeleteLeaveModal('${row.leaveId}')">
                            <i class="fas fa-trash"></i> Delete
                        </button>
                    `;
                }
                return actionButtons || `<span class="text-muted">No Actions</span>`;
            }
        });
    }

    initializeDataTable("#leaveTable", "/Leave/GetTeamLeaves", columns, {
        initComplete: function () {
            $('#startDateFilter, #endDateFilter, #statusFilter').on('change', function () {
                $('#leaveTable').DataTable().ajax.reload();
            });
        }
    });

    viewModel = new LeaveViewModel();
    ko.applyBindings(viewModel);
    ko.validation.init({
        registerExtenders: true,
        messagesOnModified: true,
        insertMessages: true,
        errorClass: 'text-danger',
        errorMessageClass: 'validationMessage',
        decorateElement: true
    });
});

function showEditLeaveModal(leaveId) {
    viewModel.openEditLeaveModal(leaveId);
}

function showDeleteLeaveModal(leaveId) {
    $("#deleteLeaveModal").modal("show");
    $("#deleteLeaveLink").data("leave-id", leaveId);
}
$(document).on('click', "#deleteLeaveLink", function (e) {
    e.preventDefault();
    const leaveId = $(this).data("leave-id");
    ajaxCall('/Leave/DeleteLeave', 'GET', JSON.stringify({ leaveId: leaveId }), function (response) {
        if (response.success) {
            $('#deleteLeaveModal').modal('hide');
            toastr.success(response.message);
            $('#leaveTable').DataTable().ajax.reload();
        } else {
            toastr.error(response.message);
        }
    });
});
