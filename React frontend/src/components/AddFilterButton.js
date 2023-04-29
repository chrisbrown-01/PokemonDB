import React from 'react';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import IconButton from '@mui/material/IconButton';

function AddFilterButton({ onClick }) {
    return (
        <IconButton color="primary" onClick={onClick}>
            <AddCircleOutlineIcon />
        </IconButton>
    )
}

export default AddFilterButton;