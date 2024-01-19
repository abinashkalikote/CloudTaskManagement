// Get all labels within the form
const labels = document.querySelectorAll('label');

// Loop through labels and check if their corresponding input fields are required
labels.forEach(label => {
    const inputId = label.getAttribute('for');
    const input = document.getElementById(inputId);

    if (input && input.hasAttribute('required')) {
        // Add an asterisk to the label text
        label.innerHTML += '  <span class="text-danger">*</span>';
    }
});



//When clicking the link button confirm button show

let TaskBtn = $(".TaskBtn");
for (let name of TaskBtn) {
    name.addEventListener('click', (e) => {
        e.preventDefault();

        if (confirm("Are you sure?")) {
            location.href = $(name).attr("href");
        } else {
            return 0;
        }
    });
}


//For Submit button disabled
$("#confirmation").submit((e) => {
    if (confirm("Are you sure ?")) {
        $('input[type=submit], button[type=submit]').prop('disabled', true);


    } else {
        e.preventDefault();
    }
});




//For ShortCut
shortcut.add("alt+H", function () { window.location.href = "/"; });
shortcut.add("alt+A", function () { window.location.href = "/Task/CreateTask"; });
shortcut.add("alt+T", function () { window.location.href = "/Task/GetAllTask"; });
shortcut.add("alt+P", function () { window.location.href = "/Task/GetAllTask?TSKStatus=Pending"; });
shortcut.add("alt+W", function () { window.location.href = "/Task/GetAllTask?TSKStatus=InProgress"; });
shortcut.add("alt+C", function () { window.location.href = "/Task/GetAllTask?TSKStatus=Completed"; });




//For chosenjs

$(".chosen-select").chosen();


////For Pagination
//const __ = document.querySelector.bind(document);
//const _a = document.querySelectorAll.bind(document)


//function previousNext(is_previous) {
//    const currentPage = parseInt($('.pagination li.current').find('span').remove().end().text());
//    let new_page_num = currentPage + 1;
//    if (is_previous == !0) {
//        new_page_num = currentPage - 1
//    }
//    navigate(new_page_num)
//}

//function navigate(new_page_num) {
//    const pageNumberQueryName = __pageNumberPrefix ? __pageNumberPrefix + ".page" : "page";
//    let parsedUrl = window.location.href;
//    const queryString = location.search;
//    const urlParams = new URLSearchParams(location.search);
//    if (queryString == "") {
//        parsedUrl += `?${pageNumberQueryName}=` + new_page_num
//    } else {
//        if (urlParams.has(pageNumberQueryName)) {
//            parsedUrl = parsedUrl.split('?')[0];
//            urlParams.delete(pageNumberQueryName);
//            parsedUrl += "?" + urlParams + `&${pageNumberQueryName}=` + new_page_num
//        } else {
//            parsedUrl += `&${pageNumberQueryName}=` + new_page_num
//        }
//    }
//    window.location.href = parsedUrl
//}

//window.__pageNumberPrefix ??= "";

//$(() => {
//    const pickers = $(".mb-nep-date-picker");

//    pickers.nepaliDatePicker({
//        dateFormat: "%y-%m-%d",
//        closeOnDateSelect: true
//    });

//    pickers.on('dateChange', function (eventData) {
//        const actualElm = __(`[name=${eventData.currentTarget.dataset.actualName}]`);
//        console.log(eventData)
//        if (!!actualElm) {
//            const date = eventData.datePickerData.adDate;
//            let value = "";
//            if (date) {
//                value = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
//            }
//            actualElm.value = value;
//        }
//    })

//    $.extend({
//        getUrlVars: function () {
//            var vars = [], hash;
//            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
//            for (var i = 0; i < hashes.length; i++) {
//                hash = hashes[i].split('=');
//                vars.push(hash[0]);
//                vars[hash[0]] = hash[1]
//            }
//            return vars
//        }, getUrlVar: function (name) {
//            return $.getUrlVars()[name]
//        }
//    });
//    $('.pagination-previous').on('click', function () {
//        $('.pagination-previous a').removeAttr('href');
//        previousNext(!0)
//    });
//    $('.pagination-next').on('click', function () {
//        $('.pagination-next a').removeAttr('href');
//        previousNext(!1)
//    });
//    $('.pagination li>a').on('click', function () {
//        $(this).removeAttr('href');
//        const pageNum = this.dataset.page ?? this.textContent;
//        navigate(pageNum)
//    });
//});