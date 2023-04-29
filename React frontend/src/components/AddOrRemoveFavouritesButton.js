import React, { useContext } from 'react';
import FavoriteIcon from '@mui/icons-material/Favorite';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import IconButton from '@mui/material/IconButton';
import { FavouritesContext } from '../modules/FavouritesContext';

function AddOrRemoveFavouritesButton({ pokedexNum }) {
    const { favourites, addFavourite, removeFavourite } = useContext(FavouritesContext);

    const isFavourite = favourites.some(item => item.pokedex_number === pokedexNum);

    function addOrRemoveFromFavourites() {
        if (isFavourite) {
            removeFavourite(pokedexNum);
        } else {
            const pokemon = {
                pokedex_number: pokedexNum,
                dateTime: new Date().toLocaleString('en-US', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    second: '2-digit'
                })
            };
            addFavourite(pokemon);
        }
    }

    return (
        <IconButton color="error" onClick={addOrRemoveFromFavourites}>
            {isFavourite ? <FavoriteIcon /> : <FavoriteBorderIcon />}
        </IconButton>
    )
}

export default AddOrRemoveFavouritesButton;