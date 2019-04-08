/**
* To Set CurrentPage
* @method SetCurrentPage
* @param {String} page - page
*/
function SetCurrentPage(page) {
    var jsonText = JSON.stringify({ 'page': page });
    $.ajax({
        type: 'POST',
        cache: false,
        url: appPath + 'UserAdmin/SetCurrentPage',
        contentType: "application/json; charset=utf-8",
        data: jsonText,
        async: false,
        error: function () {

        },
        success: function () {
        }
    });
}

/**
* To Get Current Page
* @method GetCurrentPage
*/
function GetCurrentPage() {
    var retVal = "";
    $.ajax({
        type: 'GET',
        cache: false,
        url: appPath + 'UserAdmin/GetCurrentPage',
        dataType: 'text',
        data: {},
        async: false,
        error: function () {
            retVal = "-1";
        },
        success: function (result) {
            retVal = result;
        }
    });
    return retVal;
}





