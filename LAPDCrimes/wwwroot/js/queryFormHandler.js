document.getElementById("queryForm").addEventListener("submit", function (e) {
    e.preventDefault();
    console.log("Form submitted");
    var form = e.target;
    var formData = new FormData(form);
    var queryType = document.getElementById("querySelect").value;

    fetch(`/Crime/ExecuteQuery?queryType=${queryType}`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.text())
        .then(html => {
            document.getElementById("resultsContainer").innerHTML = html;
        });
});