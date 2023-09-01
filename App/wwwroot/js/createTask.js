$(document).ready(()=>{
    $("#TaskType").change((e) => {

        if ($("#TaskType").val() == 2) {
            ChangeCloudURLLabelToGDriveLabel();
            SoftwareVersionHide();
            RemarkHide();

        } else if ($("#TaskType").val() == 3) {
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
    $("#SoftwareVersionFrom").parent("div").hide("slow");
    $("#SoftwareVersionTo").parent("div").hide("slow");
    $("#IssueOnPreviousSoftware").parent("div").hide("slow");
}

function SoftwareVersionShow() {
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