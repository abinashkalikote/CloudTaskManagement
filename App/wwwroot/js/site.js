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
            location.href = (name).attr("href");
        } else {
            return 0;
        }
    });
}
