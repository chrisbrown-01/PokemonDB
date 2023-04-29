import React, { useContext } from 'react';
import { FavouritesContext } from '../modules/FavouritesContext'
import PokemonProfiles from './PokemonProfiles';

function Favourites() {
    const { favourites } = useContext(FavouritesContext);

    return (
        <>
            <PokemonProfiles showFavouritesOnly={true} />
        </>
    )
}

export default Favourites;