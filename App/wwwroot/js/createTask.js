$(document).ready(() => {
    CreateTaskFunc();

    $("#TaskTypeId").change((e) => {
        CreateTaskFunc();
    });
    function CreateTaskFunc() {
        if ($("#TaskTypeId").val() == 2) {
            ChangeCloudURLLabelToGDriveLabel();
            CloudURLHide();
            SoftwareVersionHide();
            RemarkHide();
            PanLicDateAddressShow();

        } else if ($("#TaskTypeId").val() == 3) {
            ChangeCloudURLLabelToGDriveLabel();
            SoftwareVersionShow();
            CloudURLShow();
            RemarkShow();
            PanLicDateAddressHide();
        } else {
            ChangeGDriveLabelToCloudURLLabel();
            SoftwareVersionShow();
            CloudURLShow();
            RemarkShow();
            PanLicDateAddressHide();
        }
    }
});


function ChangeCloudURLLabelToGDriveLabel() {
    $("label[for=CloudURL]").text("Google Drive Link");
    $("#CloudURL").attr("placeholder", "Google Drive Link");
    $("#ClientName").focus();
}

function ChangeGDriveLabelToCloudURLLabel() {
    $("label[for=CloudURL]").text("Cloud URL");
    $("#CloudURL").attr("placeholder", "Cloud URL");
    $("#ClientName").focus();
}

function SoftwareVersionHide() {
    $("#SoftwareVersionFrom").removeAttr("required");
    $("#SoftwareVersionTo").removeAttr("required");
    $("#SoftwareVersionFrom").parent("div").hide("slow");
    $("#SoftwareVersionTo").parent("div").hide("slow");
    $("#IssueOnPreviousSoftware").parent("div").hide("slow");
}

function SoftwareVersionShow() {
    $("#SoftwareVersionFrom").attr("required", true);
    $("#SoftwareVersionTo").attr("required", true);
    $("#SoftwareVersionFrom").parent("div").show("slow");
    $("#SoftwareVersionTo").parent("div").show("slow");
    $("#IssueOnPreviousSoftware").parent("div").show("slow");
}

function RemarkHide() {
    $("#Remarks").parent("div").hide("slow");
}

function RemarkShow() {
    $("#Remarks").parent("div").show("slow");
}

function CloudURLHide() {
    $("#CloudURL").removeAttr("required");
    $("#CloudURL").parent("div").hide("slow");
}

function CloudURLShow() {
    $("#CloudURL").attr("required", true);
    $("#CloudURL").parent("div").show("slow");
}

function PanLicDateAddressHide() {
    //PAN Hide
    $("#PANNo").removeAttr("required");
    $("#PANNo").parent("div").hide("slow");
    //LicDate Hide
    $("#LicDate").removeAttr("required");
    $("#LicDate").parent("div").hide("slow");
    //ClientAddress Hide
    $("#ClientAddress").removeAttr("required");
    $("#ClientAddress").parent("div").hide("slow");
}

function PanLicDateAddressShow() {
    //PAN Hide
    $("#PANNo").attr("required", true);
    $("#PANNo").parent("div").show("slow");
    //LicDate Hide
    $("#LicDate").attr("required", true);
    $("#LicDate").parent("div").show("slow");
    //ClientAddress Hide
    $("#ClientAddress").attr("required", true);
    $("#ClientAddress").parent("div").show("slow");
}