/**
* @method validateMultipleDropdownSelected
* @return {bool} status
*/
function validateMultipleDropdownSelected() {
    var status = false;
    $(".selectpicker.multiple").each(function () {
        var id = $(this).attr("id");
        if ($("#" + id + " option:selected").length > 0) {
            status = true;
        }
    });
    if (status == false) {
        $('#surveyValidation').show();
        return false;
    }
    else if (status == true) {
        $('#surveyValidation').hide();
        return true;
    }
}

/**
* @method changeJewelsType
* @param {Object} sel - Dropdown object
*/
function changeJewelsType(sel) {
    var type = sel.value;
    $.ajax({
        method: "POST",
        url: appPath + "UserAdmin/GetJewelsSettingsByType/",
        data: {
            type: type,
        }
    })
        .done(function (response) {
            if (response != null) {
                $('#txtNoOfSeconds_Beg').val(response.NoOfSeconds_Beg);
                $('#txtNoOfSeconds_Adv').val(response.NoOfSeconds_Adv);
                $('#txtNoOfSeconds_Exp').val(response.NoOfSeconds_Exp);
                $('#txtNoOfSeconds_Int').val(response.NoOfSeconds_Int);
                $('#txtNoOfDiamonds').val(response.NoOfDiamonds);
                $('#txtNoOfShapes').val(response.NoOfShapes);
                $('#txtNoOfBonusPoints').val(response.NoOfBonusPoints);
                $('#txtX_NoOfChangesInLevel').val(response.X_NoOfChangesInLevel);
                $('#txtX_NoOfDiamonds').val(response.X_NoOfDiamonds);
                $('#txtY_NoOfChangesInLevel').val(response.Y_NoOfChangesInLevel);
                $('#txtY_NoOfShapes').val(response.Y_NoOfShapes);
                $('#AdminJTASettingID').val(response.AdminJTASettingID);
                $('#AdminJTBSettingID').val(response.AdminJTBSettingID);
            }

        });
}

/**
* To Save Distraction Survey
* @method SaveDistractionSurvey
*/
function SaveDistractionSurvey() {
    var status = true;
    var i = 0;
    $(".selectpicker.multiple").each(function () {
        var id = $(this).attr("id");
        jsonObj[i].SurveyArray = $(this).val();
        i++;
    });
    if (validateMultipleDropdownSelected() == false) {
        status = false;
    }
    if (status == false) {
        return false;
    }
    else {

        var postData = JSON.stringify({ 'cTestViewModelList': jsonObj });
        if (i > 0) {
            $.ajax({
                contentType: 'application/json; charset=utf-8',
                url: appPath + 'UserAdmin/SaveDistractionSurveyList',
                dataType: "json",
                type: "POST",
                data: postData,
                async: true,
                cache: false,
                traditional: true,
                success: function (data) {
                    if (data.Status == 0) {
                        $('#divNewSuccess').show();
                        setTimeout(function () { $("#divNewSuccess").hide(); }, 5000);
                        location.reload();
                    }
                },
                error: function (xhr) {

                }
            })
        }
    }

}