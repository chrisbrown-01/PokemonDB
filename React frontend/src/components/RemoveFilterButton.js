import React from 'react';
import RemoveCircleOutlineIcon from '@mui/icons-material/RemoveCircleOutline';
import IconButton from '@mui/material/IconButton';

function RemoveFilterButton({ onClick }) {
    return (
        <IconButton color="error" onClick={onClick}>
            <RemoveCircleOutlineIcon />
        </IconButton>
    )
}

export default RemoveFilterButton;