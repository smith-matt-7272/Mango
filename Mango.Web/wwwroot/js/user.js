var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        order: [[1, 'asc']],
        "ajax": { url: "/auth/getall"},
        "columns": [
            { data: 'id', "width": "25%"},
            { data: 'name', "width": "25 %"},
            { data: 'phoneNumber', "width": "25%"},
            { data: 'email', "width": "25%"}
        ]
    })
}