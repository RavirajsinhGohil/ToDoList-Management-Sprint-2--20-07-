const today = new Date().toISOString().split('T')[0];
var startDateValidation = "";
var endDateValidation = "";
var startDateValidationEdit = "";
var endDateValidationEdit = "";

let viewModel;
$(document).ready(function () {
    viewModel = new TaskViewModel();
    ko.applyBindings(viewModel);
    viewModel.loadProjects();

    initSortable();

    if (selectedProjectId) {
        viewModel.selectedProjectId(selectedProjectId);
    }
    if (selectedSprintId) {
        viewModel.selectedSprintId(selectedSprintId);
    }

    $("#addTaskModal").on('hidden.bs.modal', function () {
        $("#addTaskForm").trigger("reset");
        $("#addTaskForm").find(".text-danger").text("");
    });

    $("#addSprintModal").on('hidden.bs.modal', function () {
        $("#addSprintForm").trigger("reset");
        $("#addSprintForm").find(".text-danger").text("");
    });
});

function loadBacklogTasks(projectId) {
    ajaxCall('/Tasks/GetBacklogTasks', 'GET', JSON.stringify({ projectId: projectId }), function (html) {
        $("#backlogContainer").empty();
        $("#backlogContainer").append(html);
    });
}

function GetSprintById(sprintId) {
    ajaxCall('/Tasks/GetSprintById', 'GET', JSON.stringify({ sprintId: sprintId }), function (response) {
        if (response) {
            $("#sprintControls").empty();
            $("#sprintDuration").empty();
            if (response.status == "In Progress") {
                $("#sprintControls").append(`
                    <div class="d-flex justify-content-between">
                        <button class="btn btn-secondary" onclick="completeSprint(${sprintId})">Complete Sprint</button>
                    </div>
                `);
            }
            else if (response.status == "Not Started") {
                $("#sprintControls").append(`
                    <div class="d-flex justify-content-between">
                        <button class="btn btn-block btn-secondary text-white h-100" onclick="startSprint(${sprintId})">Start Sprint</button>
                    </div>
                `);
            }
            else if (response.status == "Completed") {
                $("#sprintControls").append(`
                    <div class="d-flex justify-content-between">
                        <button class="btn btn-secondary" disabled>Complete Sprint</button>
                    </div>
                `);
            }
            $("#sprintDuration").append(`
                <div class="d-flex justify-content-between">
                    <span >Duration: ${response.duration} days</span>
                </div>
            `);

            $("#sprintProgressBar").css("width", `${response.progress}%`);
        }
        else {
            $("#sprintControls").empty();
            $("#sprintDuration").empty();
            $("#sprintProgressBar").css("width", "0%");
            $("#sprintProgressRemaining").text("");
        }
    });
}

function renderSprintProgressBar(startDate, endDate) {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const now = new Date();

    const total = (end - start) / (1000 * 60 * 60 * 24);
    const passed = (now - start) / (1000 * 60 * 60 * 24);
    const remaining = Math.max(0, Math.ceil(total - passed));

    const percent = Math.min(100, Math.max(0, (passed / total) * 100)).toFixed(1);

    $("#sprintProgressRemaining").text(`${remaining} days remaining`);
    $("#sprintProgressBar").css("width", `${percent}%`);
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("TaskStatusChanged", (taskId, newStatus) => {
    GetTasksBySprintId(selectedProjectId);
});

connection.on("NewTaskAdded", () => {
    GetTasksBySprintId(selectedProjectId);
});

connection.on("TaskUpdated", () => {
    GetTasksBySprintId(selectedProjectId);
});

connection.on("TaskDeleted", () => {
    GetTasksBySprintId(selectedProjectId);
});

connection.start().catch(err => console.error(err.toString()));

function sendMessage() {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
}

function initSortable() {
    $(".task-column").sortable({
        connectWith: ".task-column",
        items: ".task-card",
        placeholder: "task-placeholder",
        receive: function (event, ui) {
            const taskId = ui.item.data("task-id");
            const oldStatus = ui.sender.data("status");
            const newStatus = $(this).data("status");

            const allowedTransitions = {
                "To Do": ["In Progress"],
                "In Progress": ["To Do", "Testing"],
                "Testing": ["In Progress", "Done"]
            };

            if (!allowedTransitions[oldStatus] || !allowedTransitions[oldStatus].includes(newStatus)) {
                $(ui.sender).sortable('cancel');
                toastr.warning(`Tasks from "${oldStatus}" cannot be moved to "${newStatus}".`);
                return;
            }

            ajaxCall('/Tasks/UpdateStatus', 'POST', JSON.stringify({ taskId, newStatus }), function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    GetSprintById(viewModel.selectedSprintId());
                } else {
                    toastr.error(response.message);
                    $(ui.sender).sortable('cancel');
                    return;
                }
            });
        }
    }).disableSelection();
}

function GetTasksByProjectId(selectedProjectId, userId) {
    const data = JSON.stringify({ projectId: selectedProjectId, userId: userId || 0 });

    ajaxCall('/Tasks/GetTasksByProjectId', 'GET', data, function (html) {
        $("#taskListContainer").empty().append(html);
        initSortable();
    });
}

function openAddSprintModal() {
    const projectId = viewModel.selectedProjectId();

    if (!projectId) {
        toastr.error("Please select a project");
        return;
    }

    ajaxCall('/Tasks/AddSprint', 'GET', JSON.stringify({ projectId: projectId }), function (response) {
        $("#addSprintModal").html(response);
        $("#addSprintModal").modal('show');
        $.validator.unobtrusive.parse("#addSprintForm");
    });
}

$(document).on('submit', '#addSprintForm', function (e) {
    e.preventDefault();

    const form = this;

    if (!$(form).valid()) {
        return false;
    }

    const formData = new FormData(form);

    ajaxCall('/Tasks/AddSprint', 'POST', '{}',
        function (response) {
            debugger;
            if (response.success) {
                $('#addSprintModal').modal('hide');
                toastr.success(response.message);

                viewModel.loadSprints();
            } else {
                toastr.error(response.message);
            }
        },
        {
            data: formData,
            contentType: false,
            processData: false
        }
    );
});

function openAddTaskModal() {
    const projectId = viewModel.selectedProjectId();

    if (!projectId) {
        toastr.error("Please select a project");
        return;
    }
    ajaxCall('/Tasks/AddTask', 'GET', JSON.stringify({ projectId: projectId }), function (response) {
        if (tinymce.get('addTaskDescription')) {
            tinymce.get('addTaskDescription').remove();
        }
        $("#addTaskModal").html(response);
        $("#addTaskModal").modal('show');
        $.validator.unobtrusive.parse("#addTaskForm");

        const startDateInput = $('#addTaskStartDate');

        startDateValidation = startDateInput.attr('min');
        endDateValidation = startDateInput.attr('max');
    });
}

$(document).on('submit', '#addTaskForm', function (e) {
    e.preventDefault();

    const form = this;

    if (!$(form).valid()) {
        return false;
    }

    const formData = new FormData(form);
    const selectedProjectId = viewModel.selectedProjectId();

    ajaxCall('/Tasks/AddTask', 'POST', '{}',
        function (response) {
            if (response.success) {
                $('#addTaskModal').modal('hide');
                toastr.success(response.message);

                if (selectedProjectId) {
                    loadBacklogTasks(selectedProjectId);
                }
                if (viewModel.selectedSprintId()) {
                    viewModel.loadTasks();
                    GetSprintById(viewModel.selectedSprintId());
                }
            } else {
                toastr.error(response.message);
            }
        },
        {
            data: formData,
            contentType: false,
            processData: false
        }
    );
});

function openAddTaskInputFile() {
    const fileUpload = $("#addTaskInputFile");
    fileUpload.click();
}

function openEditTaskInputFile() {
    const fileUpload = $("#editTaskInputFile");
    fileUpload.click();
}

function openEditTaskModal(taskId) {
    ajaxCall('/Tasks/GetTaskById', 'GET', JSON.stringify({ taskId: taskId }), function (response) {
        if (tinymce.get('editTaskDescription')) {
            tinymce.get('editTaskDescription').remove();
        }
        $("#editTaskModal").html(response);
        $("#editTaskModal").modal('show');
        const startDateInputEdit = $('#editTaskStartDate');

        startDateValidationEdit = startDateInputEdit.attr('min');
        endDateValidationEdit = startDateInputEdit.attr('max');

        $.validator.unobtrusive.parse("#editTaskForm");

        tinymce.init({
            selector: '#editTaskDescription',
            height: 200,
            menubar: false,
            plugins: 'lists link image',
            toolbar: 'undo redo | formatselect | bold italic underline | alignleft aligncenter alignright | bullist numlist ',
            branding: false,
            statusbar: false,
            setup: function (editor) {
                const maxLength = 1000;

                editor.on('keydown', function (e) {
                    const content = editor.getContent({ format: 'text' });

                    const navigationalKeys = [
                        'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                        'ArrowUp', 'ArrowDown', 'Control', 'Meta', 'Alt'
                    ];

                    if (content.length >= maxLength && !navigationalKeys.includes(e.key)) {
                        e.preventDefault();
                    }
                });

                editor.on('blur', function (e) {
                    const content = editor.getContent({ format: 'text' });

                    const navigationalKeys = [
                        'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                        'ArrowUp', 'ArrowDown', 'Control', 'Meta', 'Alt'
                    ];

                    if (content.length >= maxLength && !navigationalKeys.includes(e.key)) {
                        editor.setContent(content.substring(0, 1000), { format: 'raw' });
                        e.preventDefault();
                        return false;
                    }
                });
            }
        });
    });
}

function openViewTaskModal(taskId) {
    ajaxCall('/Tasks/GetTaskById', 'GET', JSON.stringify({ taskId: taskId }), function (response) {
        if (tinymce.get('editTaskDescription')) {
            tinymce.get('editTaskDescription').remove();
        }
        $("#editTaskModal").html(response);
        $("#editTaskModal").modal('show');
        const startDateInputEdit = $('#editTaskStartDate');
        $("#editViewTaskModalTitle").text("View Task");

        startDateValidationEdit = startDateInputEdit.attr('min');
        endDateValidationEdit = startDateInputEdit.attr('max');

        $("#editTaskForm").find("input, select, textarea").prop("disabled", true);
        $("#editTaskUpload").addClass('d-none');
        $("#editTaskForm").find("button[type='submit']").prop("disabled", true);
        $.validator.unobtrusive.parse("#editTaskForm");
        tinymce.init({
            selector: '#editTaskDescription',
            height: 200,
            menubar: false,
            plugins: 'lists link image',
            toolbar: 'undo redo | formatselect | bold italic underline | alignleft aligncenter alignright | bullist numlist ',
            branding: false,
            statusbar: false,
            setup: function (editor) {
                const maxLength = 1000;

                editor.on('keydown', function (e) {
                    const content = editor.getContent({ format: 'text' });

                    const navigationalKeys = [
                        'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                        'ArrowUp', 'ArrowDown', 'Control', 'Meta', 'Alt'
                    ];

                    if (content.length >= maxLength && !navigationalKeys.includes(e.key)) {
                        e.preventDefault();
                    }
                });

                editor.on('blur', function (e) {
                    const content = editor.getContent({ format: 'text' });

                    const navigationalKeys = [
                        'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
                        'ArrowUp', 'ArrowDown', 'Control', 'Meta', 'Alt'
                    ];

                    if (content.length >= maxLength && !navigationalKeys.includes(e.key)) {
                        editor.setContent(content.substring(0, 1000), { format: 'raw' });
                        e.preventDefault();
                        return false;
                    }
                });
            }
        });
    });
}

$(document).on('submit', '#editTaskForm', function (e) {
    e.preventDefault();

    const form = this;

    if (!$(form).valid()) {
        return false;
    }

    const formData = new FormData(form);

    const selectedProjectId = viewModel.selectedProjectId();
    const selectedSprintId = viewModel.selectedSprintId();

    ajaxCall('/Tasks/UpdateTask', 'POST', '{}',
        function (response) {
            if (response.success) {
                $('#editTaskModal').modal('hide');
                toastr.success(response.message);

                if (selectedProjectId) {
                    loadBacklogTasks(selectedProjectId);
                }
                if (selectedSprintId) {
                    viewModel.loadTasks();
                    GetSprintById(viewModel.selectedSprintId());
                }
            }
            else {
                toastr.error(response.message);
            }
        },
        {
            data: formData,
            contentType: false,
            processData: false
        }
    );
});

function openDeleteTaskModal(taskId) {
    $("#deleteTaskModal").modal('show');
    $("#deleteTaskLink").data("task-id", taskId);
}

$(document).on('click', '#deleteTaskLink', function (e) {
    e.preventDefault();

    const taskId = $(this).data("task-id");
    ajaxCall('/Tasks/DeleteTask', 'GET', JSON.stringify({ taskId: taskId }), function (response) {
        if (response.success) {
            $('#deleteTaskModal').modal('hide');
            toastr.success(response.message);
            const selectedProjectId = viewModel.selectedProjectId();
            const selectedSprintId = viewModel.selectedSprintId();
            if (selectedProjectId) {
                loadBacklogTasks(selectedProjectId);
            }
            if (selectedSprintId) {
                viewModel.loadTasks();
                GetSprintById(viewModel.selectedSprintId());
            }
        } else {
            toastr.error(response.message);
        }
    });
});

const maxTotalSize = 25 * 1024 * 1024;
const allowedTypes = [
    'image/jpeg', 'image/png', 'image/gif', 'image/webp',
    'application/pdf',
    'application/msword',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'text/plain'
];

$(document).on('change', '#addTaskInputFile', function () {
    const files = this.files;
    let totalSize = 0;
    const fileNamesList = $('#addTaskFileNamesList');

    for (let i = 0; i < files.length; i++) {
        totalSize += files[i].size;

        if (!allowedTypes.includes(files[i].type)) {
            toastr.error(` ${files[i].name} file is not allowed.`);
            this.value = "";
            return;
        }
        fileNamesList.append(`<div class="me-2"><i class="fa-solid fa-file"></i>${files[i].name}</div>`);
    }

    if (totalSize > maxTotalSize) {
        toastr.error('Total file size exceeds 25 MB.');
        this.value = "";
    }
});

$(document).on('change', '#editTaskInputFile', function () {
    const files = this.files;
    let totalSize = 0;
    const fileNamesList = $('#editTaskFileNamesList');

    for (let i = 0; i < files.length; i++) {
        totalSize += files[i].size;

        if (!allowedTypes.includes(files[i].type)) {
            toastr.error(`${files[i].name} file is not allowed.`);
            this.value = "";
            return;
        }
        fileNamesList.append(`<div class="me-2"><i class="fa-solid fa-file"></i>${files[i].name}</div>`);
    }

    if (totalSize > maxTotalSize) {
        toastr.error('Total file size exceeds 25 MB.');
        this.value = "";
    }
});

function startSprint(sprintId) {
    ajaxCall('/Tasks/StartSprint', 'POST', JSON.stringify({ sprintId: sprintId }), function (response) {
        if (response.success) {
            toastr.success(response.message);
            viewModel.selectedSprintId(sprintId);
            GetSprintById(sprintId);
        } else {
            toastr.error(response.message);
        }
    });
}

function completeSprint(sprintId) {
    ajaxCall('/Tasks/CompleteSprint', 'POST', JSON.stringify({ sprintId: sprintId }), function (response) {
        if (response.success) {
            toastr.success(response.message);
            viewModel.selectedSprintId(sprintId);
            GetSprintById(sprintId);
        } else {
            toastr.error(response.message);
        }
    });
}

function TaskViewModel() {
    const self = this;

    self.projects = ko.observableArray([]);
    self.sprints = ko.observableArray([]);
    self.teamMembers = ko.observableArray([]);
    self.tasks = ko.observableArray([]);

    self.selectedProjectId = ko.observable();
    self.selectedSprintId = ko.observable();
    self.selectedUserId = ko.observable();
    self.assignedToMe = ko.observable(false);

    self.loadProjects = function () {
        ajaxCall('/Tasks/GetProjectsJson', 'GET', null, function (data) {
            self.projects(data);
        });
    };

    self.loadSprints = function () {
        if (!self.selectedProjectId()) return;
        ajaxCall('/Tasks/GetSprintsJson', 'GET', JSON.stringify({ projectId: self.selectedProjectId() }), function (data) {
            self.sprints(data);
            const inProgress = data.find(x => x.status === "In Progress");
            if (inProgress) self.selectedSprintId(inProgress.sprintId);
        });
    };

    self.loadTeamMembers = function () {
        if (!self.selectedProjectId()) return;
        ajaxCall('/Tasks/GetTeamMembersJson', 'GET', JSON.stringify({ projectId: self.selectedProjectId() }), function (data) {
            self.teamMembers(data);
        });
    };

    self.loadTasks = function () {
        if (!self.selectedSprintId()) {
            ajaxCall('/Tasks/GetTasksBySprintId', 'GET', JSON.stringify({ sprintId: self.selectedSprintId() }), function (html) {
                $("#taskListContainer").html(html);
            });
        }
        const uid = self.assignedToMe() ? userId : (self.selectedUserId() || 0);
        ajaxCall('/Tasks/GetTasksBySprintId', 'GET', JSON.stringify({ sprintId: self.selectedSprintId(), userId: uid }), function (html) {
            $("#taskListContainer").html(html);
            initSortable();
        });
    };

    self.selectedProjectId.subscribe(function (newVal) {
        if (newVal) {
            self.loadSprints();
            self.loadTeamMembers();
            loadBacklogTasks(newVal);
        } else {
            self.sprints([]);
            self.teamMembers([]);
            self.tasks([]);
            loadBacklogTasks(newVal);
        }
    });

    self.selectedSprintId.subscribe(function (newSprintId) {
        self.loadTasks();

        if (newSprintId) {
            GetSprintById(newSprintId);
        } else {
            GetSprintById(null);
            $("#sprintControls").empty();
            $("#sprintDuration").empty();
        }
    });

    self.selectedUserId.subscribe(function () {
        self.loadTasks();
    });

    self.assignedToMe.subscribe(function (checked) {
        if (checked) {
            self.selectedUserId(userId);
        } else {
            self.selectedUserId(null);
        }
    });
}