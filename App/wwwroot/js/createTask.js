$(document).ready(()=>{
    $("#TaskTypeId").change((e) => {

        if ($("#TaskTypeId").val() == 2) {
            ChangeCloudURLLabelToGDriveLabel();
            SoftwareVersionHide();
            RemarkHide();

        } else if ($("#TaskTypeId").val() == 3) {
            ChangeCloudURLLabelToGDriveLabel();
            SoftwareVersionShow();
            RemarkShow();

        } else {
            ChangeGDriveLabelToCloudURLLabel();
            SoftwareVersionShow();
            RemarkShow();
        }
    });
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