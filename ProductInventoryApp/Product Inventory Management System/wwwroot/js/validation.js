// Function to validate form
function validateForm(event) {
    let isValid = true;

    const name = document.querySelector('input[name="Name"]').value;
    if (!isAlphabetic(name) && name.trim() !== '') {
        document.getElementById('nameError').innerText = 'Please enter valid name.';
        isValid = false;
    }
    
    const category = document.querySelector('input[name="Category"]').value;
    if (!isAlphabetic(category) && category.trim() !== '') {
        document.getElementById('categoryError').innerText = 'Please enter valid category name.';
        isValid = false;
    }
    
    if (!isValid) {
        event.preventDefault();
    }
}

//check is valid string
function isAlphabetic(str) {
    const regex = /^[A-Za-z]+$/;  
    return regex.test(str);
}
document.getElementById('productForm').addEventListener('submit', validateForm);