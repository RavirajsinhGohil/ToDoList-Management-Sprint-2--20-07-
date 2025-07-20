$(document).ready(function () {
    let columns = [
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
                    return data > 1 ? `${data} day(s)` : `${data} day`;
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
            searchable: true,
            searchPanes: {
                show: true
            },
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

    if (window.SelfLeavePermissions.canAddEdit || window.SelfLeavePermissions.canDelete) {
        columns.push({
            name: "Actions",
            data: "returnDate",
            title: "Actions",
            orderable: false,
            searchable: false,
            render: function (data, type, row) {
                let actionButtons = '';
                if (window.SelfLeavePermissions.canAddEdit) {
                    actionButtons += `
                    <button class="btn btn-info btn-sm text-white" onclick="showEditLeaveModal('${row.leaveId}')">
                        <i class="fas fa-pencil-alt"></i> Edit
                    </button>
                    `;
                }

                if (window.SelfLeavePermissions.canDelete) {
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

    initializeDataTable("#leaveTable", "/Leave/GetSelfLeaves", columns, {
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

    $(document).on('hidden.bs.modal', "#addLeaveModal", function () {
        var addData = viewModel.addLeaveData;
        addData.leaveId(null);
        addData.requestedId(null);
        addData.approvalId(null);
        addData.requestedName("");
        addData.approvalName("");
        addData.startDate(null);
        addData.endDate(null);
        addData.reason("");
        addData.duration(null);
        addData.returnDate(null);
        addData.phoneNumber("");
        addData.alternatePhoneNumber("");
        addData.availableOnPhone(null);
        addData.approvedDate(null);

        var errors = ko.validation.group(addData);
        errors.showAllMessages(false);
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