function LeaveViewModel() {
    var self = this;
    self.availableOnPhoneOptions = ko.observableArray([
        { id: true, name: "Yes" },
        { id: false, name: "No" },
    ]);
    self.statusFilterOptions = ko.observableArray([
        { id: 0, name: "All" },
        { id: 1, name: "Pending" },
        { id: 2, name: "Approved" },
        { id: 3, name: "Rejected" }
    ]);
    self.statusOptions = ko.observableArray([
        { id: 0, name: "Pending" },
        { id: 1, name: "Approved" },
        { id: 2, name: "Rejected" }
    ]);

    self.addLeaveData = {
        leaveId: ko.observable(),
        requestedId: ko.observable(),
        approvalId: ko.observable(),
        requestedName: ko.observable().extend({ required: true, maxLength: 50, required: { message: "Name is required." } }),
        approvalName: ko.observable().extend({ required: true, maxLength: 50, required: { message: "Reporting Person is required." } }),
        startDate: ko.observable().extend({ required: true, required: { message: "Start Date is required." } }),
        endDate: ko.observable().extend({ required: true, required: { message: "End Date is required." } }),
        reason: ko.observable().extend({ required: true, maxLength: 1000, required: { message: "Reason is required." } }),
        duration: ko.observable(),
        returnDate: ko.observable(),
        phoneNumber: ko.observable().extend({ required: true, required: { message: "Phone Number is required." }, pattern: { message: "Invalid Phone Number.", params: "^[0-9]{10}$" } }),
        alternatePhoneNumber: ko.observable().extend({ pattern: { message: "Invalid Phone Number.", params: "^[0-9]{10}$" } }),
        availableOnPhone: ko.observable(),
        approvedDate: ko.observable()
    };

    self.editLeaveData = {
        leaveId: ko.observable(),
        requestedId: ko.observable(),
        approvalId: ko.observable(),
        requestedName: ko.observable().extend({ required: true, maxLength: 50, required: { message: "Name is required." } }),
        approvalName: ko.observable().extend({ required: true, maxLength: 50, required: { message: "Reporting Person is required." } }),
        startDate: ko.observable().extend({ required: true, required: { message: "Start Date is required." } }),
        endDate: ko.observable().extend({ required: true, required: { message: "End Date is required." } }),
        reason: ko.observable().extend({ required: true, maxLength: 1000, required: { message: "Reason is required." } }),
        duration: ko.observable(),
        returnDate: ko.observable(),
        phoneNumber: ko.observable().extend({ required: true, required: { message: "Phone Number is required." }, pattern: { message: "Invalid Phone Number.", params: "^[0-9]{10}$" } }),
        alternatePhoneNumber: ko.observable().extend({ pattern: { message: "Invalid Phone Number.", params: "^[0-9]{10}$" } }),
        availableOnPhone: ko.observable(),
        approvedDate: ko.observable(),
        status: ko.observable(),
        commentByApproval: ko.observable()
    };

    const today = new Date().toISOString().split('T')[0];
    self.addLeaveData.startDate.subscribe(function (newValue) {
        if (!newValue) {
            $("#addLeaveEndDate").attr("min", today);
        }
        else {
            $("#addLeaveEndDate").attr("min", newValue);
        }
        self.addLeaveData.durationCalculate();
    });

    self.addLeaveData.endDate.subscribe(function (newValue) {
        if (!newValue) {
            $("#addLeaveStartDate").removeAttr("max");
        }
        else {
            $("#addLeaveStartDate").attr("max", newValue);
        }
        self.addLeaveData.durationCalculate();
    });

    self.addLeaveData.durationCalculate = ko.computed(function () {
        var startDate = self.addLeaveData.startDate();
        var endDate = self.addLeaveData.endDate();
        if (startDate && endDate) {
            var start = new Date(startDate);
            var end = new Date(endDate);
            var duration = Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;
            self.addLeaveData.duration(duration);
            self.addLeaveData.returnDate(new Date(end.setDate(end.getDate() + 1)).toISOString().split('T')[0]);
            return duration;
        }
        return 0;
    });

    self.editLeaveData.startDate.subscribe(function (newValue) {
        if (!newValue) {
            $("#editLeaveEndDate").attr("min", today);
        }
        else {
            $("#editLeaveEndDate").attr("min", newValue);
        }
        self.editLeaveData.durationCalculate();
    });

    self.editLeaveData.endDate.subscribe(function (newValue) {
        if (!newValue) {
            $("#editLeaveStartDate").removeAttr("max");
        }
        else {
            $("#editLeaveStartDate").attr("max", newValue);
        }
        self.editLeaveData.durationCalculate();
    });

    self.editLeaveData.durationCalculate = ko.computed(function () {
        var startDate = self.editLeaveData.startDate();
        var endDate = self.editLeaveData.endDate();
        if (startDate && endDate) {
            var start = new Date(startDate);
            var end = new Date(endDate);
            var duration = Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;
            self.editLeaveData.duration(duration);
            self.editLeaveData.returnDate(new Date(end.setDate(end.getDate() + 1)).toISOString().split('T')[0]);
            return duration;
        }
        return 0;
    });

    self.openAddLeaveModal = function () {
        debugger;
        ajaxCall('/Leave/AddLeave', 'GET',  null, function (response) {
            if (response.success) {
                self.addLeaveData.requestedName(response.data.requestedName);
                self.addLeaveData.approvalName(response.data.requestedName);
                console.log(self.addLeaveData);
                $('#addLeaveModal').modal('show');
            }
            else {
                toastr.error(response.message);
                return;
            }
        });
        $.validator.unobtrusive.parse("#addLeaveForm");
    };

    self.submitAddLeaveForm = function () {
        var errors = ko.validation.group(self.addLeaveData);
        if (errors().length === 0) {
            console.log("Form submitted successfully!");
        } else {
            console.log(errors());
            errors.showAllMessages();
            return;
        }

        const data = {
            requestedUserId: self.addLeaveData.requestedId(),
            approvalId: self.addLeaveData.approvalId(),
            requestedName: self.addLeaveData.requestedName(),
            approvalName: self.addLeaveData.approvalName(),
            reason: self.addLeaveData.reason(),
            startDate: self.addLeaveData.startDate(),
            endDate: self.addLeaveData.endDate(),
            phoneNumber: self.addLeaveData.phoneNumber(),
            alternatePhoneNumber:self.addLeaveData.alternatePhoneNumber(),
            isAvailableOnPhone: self.addLeaveData.availableOnPhone()
        };
        $.post("/Leave/AddLeave", data, function (response) {
            $('#addLeaveModal').modal('hide');
            toastr.success(response.message);
            $('#leaveTable').DataTable().ajax.reload();
        });
    };

    self.openEditLeaveModal = function (leaveId) {
        ajaxCall('/Leave/GetLeaveById', 'GET',  JSON.stringify({ leaveId: leaveId }), function (response) {
            let data = response.data;
            self.editLeaveData.leaveId(data.leaveId);
            self.editLeaveData.requestedId(data.requestedUserId);
            self.editLeaveData.approvalId(data.approvalUserId);
            self.editLeaveData.requestedName(data.requestedName);
            self.editLeaveData.approvalName(data.approvalName);
            self.editLeaveData.reason(data.reason);
            self.editLeaveData.phoneNumber(data.phoneNumber);
            self.editLeaveData.alternatePhoneNumber(data.alternatePhoneNumber);
            self.editLeaveData.startDate(data.startDate);
            self.editLeaveData.endDate(data.endDate);
            self.editLeaveData.duration(data.duration);
            self.editLeaveData.returnDate(data.returnDate);
            self.editLeaveData.availableOnPhone(data.availableOnPhone);
            self.editLeaveData.approvedDate(data.approvedOn);
            self.editLeaveData.status(data.status);
            self.editLeaveData.commentByApproval(data.commentByApproval);
            console.log(self.editLeaveData);
            $("#editLeaveStartDate").attr("max", self.editLeaveData.startDate());
            $("#editLeaveEndDate").attr("min", self.editLeaveData.startDate());
            $("#editLeaveModal").modal("show");
        });
    };

    self.submitEditLeaveForm = function () {
        var errors = ko.validation.group(self.editLeaveData);
        if (errors().length === 0) {
            console.log("Form submitted successfully!");
        } else {
            console.log(errors());
            errors.showAllMessages();
            return;
        }

        const data = {
            leaveId: self.editLeaveData.leaveId(),
            requestedUserId: self.editLeaveData.requestedId(),
            approvalUserId: self.editLeaveData.approvalId(),
            requestedName: self.editLeaveData.requestedName(),
            approvalName: self.editLeaveData.approvalName(),
            reason: self.editLeaveData.reason(),
            startDate: self.editLeaveData.startDate(),
            endDate: self.editLeaveData.endDate(),
            phoneNumber: self.editLeaveData.phoneNumber(),
            alternatePhoneNumber:self.editLeaveData.alternatePhoneNumber(),
            isAvailableOnPhone: self.editLeaveData.availableOnPhone(),
            status: self.editLeaveData.status(),
            commentByApproval: self.editLeaveData.commentByApproval()
        };

        $.post("/Leave/UpdateLeave", data, function (response) {
            if(response.success) {
                $('#editLeaveModal').modal('hide');
                toastr.success(response.message);
                $('#leaveTable').DataTable().ajax.reload();
            }
            else {
                toastr.error(response.message);
            }
        });
    };
}