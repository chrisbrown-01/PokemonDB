import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { Button } from '@mui/material';
import TableComponent from './TableComponent';
import SearchBar from './SearchBar';

function PokemonStats() {
    const [pokemonStats, setPokemonStats] = useState(null);
    const [pokemonStats_Compare, setPokemonStats_Compare] = useState(null);
    const [compareBool, setCompareBool] = useState(false);
    const [showSearch, setShowSearch] = useState(false);

    const params = useParams();
    const pokemonName = params.name;

    const searchBarItemSelected = async (pokedex_number) => {
        try {
            const response = await fetch(`http://localhost:55558/api/pokemon/pokedexnumber/${pokedex_number}`);
            if (response.status === 200) {
                const data = await response.json();
                setPokemonStats_Compare(data);
                setCompareBool(true);
            } else {
                console.log(`Request failed with status code ${response.status}`);
                setPokemonStats_Compare(null);
                setCompareBool(false);
            }
        } catch (error) {
            console.log(error.message);
            setPokemonStats_Compare(null);
            setCompareBool(false);
        }
    }

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(`http://localhost:55558/api/pokemon/name/${pokemonName}`);
                if (response.status === 200) {
                    const data = await response.json();
                    setPokemonStats(data);
                } else {
                    console.log(`Request failed with status code ${response.status}`);
                    setPokemonStats(null);
                }
            } catch (error) {
                console.log(error.message);
                setPokemonStats(null);
            }
        };
        fetchData();
    }, []);

    return (
        <div className="center">
            {compareBool ?
                (
                    <div>
                        <div className="input-and-button-spacing">
                            <Button color="primary" variant="contained" onClick={() => { setCompareBool(!compareBool); setShowSearch(!showSearch) }}>Remove comparison</Button>
                        </div>
                        {pokemonStats && pokemonStats_Compare && <TableComponent pokemon1={pokemonStats} pokemon2={pokemonStats_Compare} />}
                    </div>
                )
                :
                (
                    <div>
                        <div className="input-and-button-spacing">
                            {
                                showSearch ?
                                    (
                                        <SearchBar onListItemClick={(pokemon) => searchBarItemSelected(pokemon.pokedex_number)} />
                                    )
                                    :
                                    (
                                        <Button color="primary" variant="contained" onClick={() => setShowSearch(!showSearch)}>Click to compare</Button>
                                    )
                            }
                        </div>
                        <div>
                            {pokemonStats && <TableComponent pokemon1={pokemonStats} />}
                        </div>
                    </div>
                )
            }
        </div>

    );
}

export default PokemonStats;