function TeamViewModel() {
    var self = this;

    self.teamMemberData = {
        userId: ko.observable(),
        name: ko.observable(),
        email: ko.observable(),
        roleId: ko.observable(),
        roleName: ko.observable(),
        phoneNumber: ko.observable()
    }

    self.addTeamMembersData = {
        teamMembers: ko.observableArray([]),
        notAssignedMembers: ko.observableArray([]),
        selectedNotAssignedMembers: ko.observableArray([]).extend({ required: true, required: { message: "Select User(s) is required." } }),
        teamMember: ko.observable()
    }

    self.editTeamMembersData = {
        teamMember: ko.observable(),
        teamManagers: ko.observableArray([]),
        selectedTeamManager: ko.observable().extend({ required: true, required: { message: "Team Manager is required." } })
    }

    self.shouldShowHierarchy = ko.observable(true);
    self.shouldShowMembers = ko.observable(false);

    self.removeMemberId = ko.observable();

    self.teams = ko.observableArray([]);

    self.openAddTeamMembersModal = function () {
        debugger;
        ajaxCall('/Team/GetTeamMembersJson', 'GET', null, function (data) {
            self.addTeamMembersData.teamMembers(data);
        });
        ajaxCall('/Team/GetNotAssignedMembers', 'GET', null, function (data) {
            self.addTeamMembersData.notAssignedMembers(data);
            console.log(data);
            $('#addTeamMembersModal').modal('show');
        });
    };

    self.addTeamMembersData.selectedNotAssignedMembers.subscribe(function () {
        var length = self.addTeamMembersData.selectedNotAssignedMembers().length;
        if (length != 0) {
            $("#addMembersDropDownTitle").text(`${length} selected`);
        }
        else {
            $("#addMembersDropDownTitle").text('Select User(s)*');
        }
    });

    self.submitAddMembersForm = function () {
        var errors = ko.validation.group(self.addTeamMembersData);
        if (errors().length === 0) {
            console.log("Form submitted successfully!");
        } else {
            console.log(errors());
            errors.showAllMessages();
            return;
        }
        const data = {
            teamManagerId: self.addTeamMembersData.teamMember
        };

        var members = self.addTeamMembersData.selectedNotAssignedMembers();

        if (Array.isArray(members)) {
            members.forEach(function (id, i) {
                data[`teamMemberIds[${i}]`] = id;
            });
        } else {
            console.warn('selectedNotAssignedMembers is not an array');
        }

        $.post("/Team/AddTeamMembers", data, function (response) {
            if (response.success) {
                $('#addTeamMembersModal').modal('hide');
                toastr.success(response.message);
                $('#teamMembersTable').DataTable().ajax.reload();
            }
            else {
                toastr.error(response.message);
            }
        });
        console.log(self.addTeamMembersData.selectedNotAssignedMembers);
    };

    self.openChangeTeamMemberModal = function (teamMemberId) {
        ajaxCall('/Team/GetTeamMemberById', 'GET', JSON.stringify({ teamMemberId: teamMemberId }) , function (response) {
            var data = response.data;
            self.editTeamMembersData.teamManagers(data.teamManagers);
            self.editTeamMembersData.teamMember(data.teamMember);
            self.editTeamMembersData.selectedTeamManager(data.selectedTeamManagerId);
            $('#changeTeamMemberModal').modal('show');
        });
    }

    self.submitChangeTeamMembersForm = function () {
        var errors = ko.validation.group(self.editTeamMembersData);
        if (errors().length === 0) {
            console.log("Form submitted successfully!");
        } else {
            console.log(errors());
            errors.showAllMessages();
            return;
        }

        const data = {
            teamMember: self.editTeamMembersData.teamMember(),
            selectedTeamManagerId: self.editTeamMembersData.selectedTeamManager()
        };

        $.post("/Team/ChangeTeamMember", data, function (response) {
            if (response.success) {
                $('#changeTeamMemberModal').modal('hide');
                toastr.success(response.message);
                $('#teamMembersTable').DataTable().ajax.reload();
            }
            else {
                toastr.error(response.message);
            }
        });
    }

    self.openRemoveMemberModal = function(memberId) {
        self.removeMemberId(memberId);
        $("#removeMemberModal").modal('show');
    }

    self.removeMemberFromTeam = function() {
        const teamMemberId = self.removeMemberId();
        ajaxCall("/Team/RemoveTeamMember", 'GET', JSON.stringify({ teamMemberId: teamMemberId }), function (response) {
            if (response.success) {
                $("#removeMemberModal").modal('hide');
                toastr.success(response.message);
                $('#teamMembersTable').DataTable().ajax.reload();
            }
            else {
                toastr.error(response.message);
            }
        });
    }

    self.root = ko.observable(new Node("Root", []));

    self.loadTeams = function () {
        ajaxCall('/Team/GetTeamsList', 'GET', null, function (response) {
            // Assuming response is an array of TeamViewModel objects
            var teams = response.map(function (teamData) {
                return new Node(
                    teamData.teamManager.name,
                    teamData.teamMembers.map(function (member) {
                        return new Node(member.name);  // Create a Node for each team member
                    })
                );
            });

            self.teams(teams);  // Set the teams array with the hierarchical data
        });
    };

    self.loadTeams();

    self.showTeamHierarchy = function () {
        debugger;
        self.shouldShowHierarchy(false);
        self.shouldShowMembers(true);
        self.loadTeams();
    };

    self.showTeamMembers = function () {
        debugger;
        self.shouldShowMembers(false);
        self.shouldShowHierarchy(true);
    };
}

function Node(name, children = []) {
    this.name = name;
    this.children = ko.observableArray(children);  // Children of this node
    this.collapsed = ko.observable(true);  // Controls visibility of children (team members)
    
    // Toggle the collapsed state
    this.toggle = function () {
        this.collapsed(!this.collapsed());  
    };
}