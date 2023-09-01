var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDataTable("approved");
    } else if (url.includes("readyforpickup")) {
        loadDataTable("readyforpickup");
    } else if (url.includes("cancelled")) {
        loadDataTable("cancelled");
    } else {
        loadDataTable("all");
    }

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        order: [[0, 'desc']],
        "ajax": { url: "/order/getall?status=" + status },
        "columns": [
            { data: 'orderHeaderID', "width": "5%"},
            { data: 'email', "width": "25 %"},
            { data: 'lastName', "width": "10%" },
            { data: 'firstName', "width": "15%" },
            { data: 'phone', "width": "20%"},
            { data: 'status', "width": "15%"},
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'orderHeaderID',
                "render": function (data) {
                    return `<div class="w-75 brn-group" role="group">
                        <a href="/order/orderDetails?orderID=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                }, width:"10%"
            }
        ]
    })
}