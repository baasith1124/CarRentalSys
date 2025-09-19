// KYC Upload Modal JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('kycUploadForm');
    const fileUploadArea = document.getElementById('fileUploadArea');
    const fileInput = document.getElementById('documentFile');
    const filePreview = document.getElementById('filePreview');
    const previewFileName = document.getElementById('previewFileName');
    const previewFileSize = document.getElementById('previewFileSize');
    const removeFileBtn = document.getElementById('removeFile');
    
    if (!form) return;
    
    // Initialize button state
    updateButtonState();
    
    // Prevent default drag and drop behavior on the entire document
    document.addEventListener('dragover', (e) => {
        e.preventDefault();
    });
    
    document.addEventListener('drop', (e) => {
        e.preventDefault();
    });
    
    // File upload area click handler
    fileUploadArea.addEventListener('click', (e) => {
        // Only trigger if not clicking on the file input itself
        if (e.target !== fileInput) {
            fileInput.click();
        }
    });
    
    // Drag and drop functionality
    fileUploadArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        e.stopPropagation();
        fileUploadArea.classList.add('dragover');
        console.log('Drag over detected');
    });
    
    fileUploadArea.addEventListener('dragenter', (e) => {
        e.preventDefault();
        e.stopPropagation();
        fileUploadArea.classList.add('dragover');
        console.log('Drag enter detected');
    });
    
    fileUploadArea.addEventListener('dragleave', (e) => {
        e.preventDefault();
        e.stopPropagation();
        console.log('Drag leave detected');
        // Only remove dragover if leaving the upload area completely
        if (!fileUploadArea.contains(e.relatedTarget)) {
            fileUploadArea.classList.remove('dragover');
        }
    });
    
    fileUploadArea.addEventListener('drop', (e) => {
        e.preventDefault();
        e.stopPropagation();
        fileUploadArea.classList.remove('dragover');
        
        console.log('Drop event triggered');
        const files = e.dataTransfer.files;
        console.log('Files dropped:', files.length, files);
        
        if (files.length > 0) {
            console.log('Processing dropped file:', files[0].name);
            handleFileSelect(files[0]);
        } else {
            console.log('No files in drop event');
        }
    });
    
    // File input change handler
    fileInput.addEventListener('change', (e) => {
        console.log('File input changed:', e.target.files.length);
        if (e.target.files.length > 0) {
            handleFileSelect(e.target.files[0]);
        }
    });
    
    // Remove file handler
    removeFileBtn.addEventListener('click', () => {
        clearFileSelection();
    });
    
    // Document type selection handlers
    const documentTypeRadios = document.querySelectorAll('input[name="DocumentType"]');
    documentTypeRadios.forEach(radio => {
        radio.addEventListener('change', () => {
            console.log('Document type selected:', radio.value);
            clearFieldError('documentTypeError');
            updateButtonState();
        });
    });
    
    // Form submission handler
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        if (validateForm()) {
            submitKYCForm();
        }
    });
    
    function handleFileSelect(file) {
        console.log('File selected:', {
            name: file.name,
            size: file.size,
            type: file.type
        });
        
        // Validate file
        if (!validateFile(file)) {
            console.log('File validation failed');
            return;
        }
        
        console.log('File validation passed');
        
        // Show preview
        previewFileName.textContent = file.name;
        previewFileSize.textContent = formatFileSize(file.size);
        
        fileUploadArea.style.display = 'none';
        filePreview.classList.remove('d-none');
        
        // Clear any previous errors
        clearFieldError('documentFileError');
        
        // Update button state
        updateButtonState();
    }
    
    function clearFileSelection() {
        fileInput.value = '';
        fileUploadArea.style.display = 'block';
        filePreview.classList.add('d-none');
        clearFieldError('documentFileError');
        updateButtonState();
    }
    
    function updateButtonState() {
        const submitButton = form.querySelector('.btn-upload-3d');
        const documentType = document.querySelector('input[name="DocumentType"]:checked');
        const hasFile = fileInput.files && fileInput.files.length > 0;
        
        console.log('Updating button state:', {
            hasDocumentType: !!documentType,
            hasFile: hasFile,
            documentTypeValue: documentType?.value
        });
        
        if (submitButton) {
            if (documentType && hasFile) {
                submitButton.disabled = false;
                submitButton.style.opacity = '1';
                console.log('Button enabled');
            } else {
                submitButton.disabled = true;
                submitButton.style.opacity = '0.6';
                console.log('Button disabled');
            }
        }
    }
    
    function validateFile(file) {
        const maxSize = 5 * 1024 * 1024; // 5MB
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'application/pdf'];
        const allowedExtensions = ['.jpg', '.jpeg', '.png', '.pdf'];
        
        console.log('Validating file:', {
            name: file.name,
            size: file.size,
            type: file.type
        });
        
        // Check file size
        if (file.size > maxSize) {
            console.log('File too large:', file.size, 'bytes');
            showFieldError('documentFileError', 'File size must be less than 5MB');
            return false;
        }
        
        // Check file extension (more lenient)
        const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
        console.log('File extension:', fileExtension);
        
        if (!allowedExtensions.includes(fileExtension)) {
            console.log('Invalid file extension:', fileExtension);
            showFieldError('documentFileError', 'Only JPG, PNG, and PDF files are allowed');
            return false;
        }
        
        console.log('File validation passed');
        return true;
    }
    
    function validateForm() {
        let isValid = true;
        
        // Validate document type
        const documentType = document.querySelector('input[name="DocumentType"]:checked');
        if (!documentType) {
            showFieldError('documentTypeError', 'Please select a document type');
            isValid = false;
        } else {
            clearFieldError('documentTypeError');
        }
        
        // Validate file
        if (!fileInput.files || fileInput.files.length === 0) {
            showFieldError('documentFileError', 'Please select a file to upload');
            isValid = false;
        } else {
            clearFieldError('documentFileError');
        }
        
        return isValid;
    }
    
    function showFieldError(fieldId, message) {
        const errorElement = document.getElementById(fieldId);
        if (errorElement) {
            errorElement.textContent = message;
        }
    }
    
    function clearFieldError(fieldId) {
        const errorElement = document.getElementById(fieldId);
        if (errorElement) {
            errorElement.textContent = '';
        }
    }
    
    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }
    
    function submitKYCForm() {
        const submitButton = form.querySelector('.btn-upload-3d');
        const errorDisplay = document.getElementById('kycUploadError');
        
        // Show loading state
        const originalText = submitButton.querySelector('.btn-text').textContent;
        submitButton.querySelector('.btn-text').textContent = 'Uploading...';
        submitButton.disabled = true;
        
        // Clear previous errors
        if (errorDisplay) {
            errorDisplay.style.display = 'none';
            errorDisplay.textContent = '';
        }
        
        // Create FormData manually to ensure proper file handling
        const formData = new FormData();
        const documentType = document.querySelector('input[name="DocumentType"]:checked');
        const file = fileInput.files[0];
        
        console.log('Form data preparation:', {
            documentType: documentType?.value,
            file: file?.name,
            fileSize: file?.size,
            fileType: file?.type
        });
        
        if (!documentType) {
            throw new Error('Please select a document type');
        }
        
        if (!file) {
            throw new Error('Please select a file to upload');
        }
        
        formData.append('DocumentType', documentType.value);
        formData.append('Document', file);
        
        // Add anti-forgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        if (token) {
            formData.append('__RequestVerificationToken', token.value);
        }
        
        // Log FormData contents
        console.log('FormData entries:');
        for (let [key, value] of formData.entries()) {
            console.log(key, value);
        }
        
        console.log('Submitting KYC form with:', {
            documentType: documentType?.value,
            fileName: file?.name,
            fileSize: file?.size
        });
        
        console.log('Making fetch request to /KYC/UploadAjax');
        console.log('FormData contents:');
        for (let [key, value] of formData.entries()) {
            console.log(`  ${key}:`, value);
        }
        
        fetch('/KYC/UploadAjax', {
            method: 'POST',
            body: formData
        })
        .then(response => {
            console.log('Response status:', response.status);
            console.log('Response headers:', response.headers);
            
            if (response.ok) {
                return response.json();
            } else {
                return response.text().then(text => {
                    console.log('Error response text:', text);
                    console.log('Response status:', response.status);
                    console.log('Response statusText:', response.statusText);
                    
                    try {
                        const data = JSON.parse(text);
                        console.log('Parsed error data:', data);
                        const errorMessage = data.message || data.errors?.join(', ') || `Server error (${response.status}): ${response.statusText}`;
                        throw new Error(errorMessage);
                    } catch (e) {
                        console.log('Error parsing response:', e);
                        console.log('Raw response text:', text);
                        const errorMessage = `Server error (${response.status}): ${response.statusText}. Response: ${text}`;
                        throw new Error(errorMessage);
                    }
                });
            }
        })
        .then(data => {
            if (data.success) {
                // Success - close modal and show success message
                const modal = bootstrap.Modal.getInstance(document.getElementById('kycUploadModal'));
                if (modal) {
                    modal.hide();
                }
                
                showSuccessMessage('KYC document uploaded successfully! Your verification is pending admin approval.');
                
                // Refresh the page to update KYC status
                setTimeout(() => {
                    window.location.reload();
                }, 2000);
            } else {
                throw new Error(data.message || 'Upload failed');
            }
        })
        .catch(error => {
            console.error('KYC upload error:', error);
            console.error('Error stack:', error.stack);
            showKYCErrors([error.message || 'Upload failed. Please try again.']);
        })
        .finally(() => {
            // Reset button state
            submitButton.querySelector('.btn-text').textContent = originalText;
            submitButton.disabled = false;
        });
    }
    
    function showKYCErrors(errors) {
        const errorDisplay = document.getElementById('kycUploadError');
        if (errorDisplay) {
            errorDisplay.innerHTML = errors.map(error => `<div>${error}</div>`).join('');
            errorDisplay.style.display = 'block';
        }
    }
    
    function showSuccessMessage(message) {
        // Create a temporary success message
        const successDiv = document.createElement('div');
        successDiv.className = 'alert alert-success position-fixed';
        successDiv.style.cssText = `
            top: 20px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            background: linear-gradient(135deg, rgba(40, 167, 69, 0.95), rgba(25, 135, 84, 0.95));
            border: none;
            color: white;
            border-radius: 15px;
            backdrop-filter: blur(10px);
        `;
        successDiv.innerHTML = `
            <div class="d-flex align-items-center">
                <i class="bi bi-check-circle-fill me-2"></i>
                <span>${message}</span>
            </div>
        `;
        
        document.body.appendChild(successDiv);
        
        // Remove after 4 seconds
        setTimeout(() => {
            successDiv.remove();
        }, 4000);
    }
});

// Modal event handlers
document.addEventListener('DOMContentLoaded', function() {
    const kycModal = document.getElementById('kycUploadModal');
    if (kycModal) {
        // Reset form when modal is hidden
        kycModal.addEventListener('hidden.bs.modal', function() {
            const form = document.getElementById('kycUploadForm');
            if (form) {
                form.reset();
                // Clear file preview
                const fileUploadArea = document.getElementById('fileUploadArea');
                const filePreview = document.getElementById('filePreview');
                if (fileUploadArea && filePreview) {
                    fileUploadArea.style.display = 'block';
                    filePreview.classList.add('d-none');
                }
                // Clear all validation errors
                const errorElements = form.querySelectorAll('.validation-error-3d');
                errorElements.forEach(element => {
                    element.textContent = '';
                });
                // Hide error display
                const errorDisplay = document.getElementById('kycUploadError');
                if (errorDisplay) {
                    errorDisplay.style.display = 'none';
                }
            }
        });
        
        // Focus first input when modal is shown
        kycModal.addEventListener('shown.bs.modal', function() {
            const firstInput = kycModal.querySelector('input[type="radio"]');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }
});

// Global function to show KYC upload modal
function showKYCUploadModal() {
    const modal = new bootstrap.Modal(document.getElementById('kycUploadModal'));
    modal.show();
}
