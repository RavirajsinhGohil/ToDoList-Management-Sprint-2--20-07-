$(document).ready(function () {
    $("#addRoleModal").on('hidden.bs.modal', function () {
        $("#addTaskForm").trigger("reset");
        $("#addTaskForm").find(".text-danger").text("");
    });
});

function openEditRoleModal(roleId) {
    var data = JSON.stringify({ roleId: roleId });
    ajaxCall('/RoleAndPermission/GetRoleById', 'GET', data, function (data) {
        $("#editRoleModal").empty();
            $("#editRoleModal").append(data);
            $("#editRoleModal").modal('show');
            $.validator.unobtrusive.parse("#editRoleForm");
    });
}

function openDeleteRoleModal(roleId) {
    $("#deleteRoleModal").modal('show');
    $("#deleteRoleLink").attr("href", `/RoleAndPermission/DeleteRole?roleId=${roleId}`);
}