// alerts.js
function showAlert(message, type) {
    switch (type) {
        case 'success':
            alert('Success: ' + message);
            break;
        case 'error':
            alert('Error: ' + message);
            break;
        case 'warning':
            alert('Warning: ' + message);
            break;
        default:
            alert(message);
    }
}
