let form = document.querySelector('#formdata');

form.addEventListener('submit', async function (e) {
    e.preventDefault();
    let data = new FormData(form);
    let response = await fetch('http://localhost:51090/Customer/displaynew', {
        method: 'POST',
        headers: {
            'Content-type': 'application/json'
        },
        body: data,
        mode: "no-cors"
    });
    let responseData = await response.json();
});