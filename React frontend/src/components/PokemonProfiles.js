import React, { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import './PokemonProfiles.css';
import TablePagination from '@mui/material/TablePagination';
import AddOrRemoveFavouritesButton from './AddOrRemoveFavouritesButton';
import { FavouritesContext } from '../modules/FavouritesContext'

function PokemonProfiles({ showFavouritesOnly }) {
    const [profileInfo, setprofileInfo] = useState([]);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const { favourites } = useContext(FavouritesContext);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('http://localhost:55558/api/pokemon/namesNumsImages');
                if (response.status === 200) {
                    const data = await response.json();

                    if (showFavouritesOnly) {
                        const favData = data.filter(profile => favourites.some(favourite => favourite.pokedex_number === profile.pokedex_number));
                        setprofileInfo(favData);
                    }
                    else {
                        setprofileInfo(data);
                    }
                } else {
                    console.log(`Request failed with status code ${response.status}`);
                    setprofileInfo([]);
                }
            } catch (error) {
                console.log(error.message);
                setprofileInfo([]);
            }
        };
        fetchData();
    }, []);

    const profiles = profileInfo
        .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
        .map((profile, key) =>
            <div className="profiles-item" key={key}>
                {profile.pokedex_number}.
                {" "}
                {profile.name}
                <AddOrRemoveFavouritesButton pokedexNum={profile.pokedex_number} />
                <img
                    src={`data:image/${profile.fileName.split('.').pop()};base64,${profile.imageBase64Data}`} alt={profile.fileName}
                    onClick={() => navigate(`/pokemon-stats/${profile.name}`)}
                />
            </div>
        );

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    return (
        <div>
            <div className="center">
                {showFavouritesOnly ? <h1>Favourites</h1> : <h1>Pokemon Profiles</h1>}
            </div>
            <TablePagination
                component="div"
                count={profileInfo.length}
                page={page}
                onPageChange={handleChangePage}
                rowsPerPage={rowsPerPage}
                onRowsPerPageChange={handleChangeRowsPerPage}
            />
            <div className="profiles-container">
                {profiles}
            </div>
        </div>

    );
}

export default PokemonProfiles;