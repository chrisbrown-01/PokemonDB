import React from 'react';
import './TableComponent.css'
import AddOrRemoveFavouritesButton from './AddOrRemoveFavouritesButton';
import { keyOrder, pokemonProperties_Pretty, damageKeys } from './Data/PropertyAndFilteringLists'

const TableComponent = ({ pokemon1, pokemon2 }) => {
    const prettifyPokemonProp = (pokemonApiResponse) => {
        let pretty = pokemonApiResponse;

        if (pretty.type1 != null && pretty.type1 != undefined) {
            pretty.type1 = pretty.type1.charAt(0).toUpperCase() + pretty.type1.slice(1);
        }

        if (pretty.type2 != null && pretty.type2 != undefined) {
            pretty.type2 = pretty.type2.charAt(0).toUpperCase() + pretty.type2.slice(1);
        }

        if (pretty.is_legendary != null && pretty.is_legendary != undefined) {
            pretty.is_legendary = pretty.is_legendary.toString().toUpperCase();
        }

        return pretty;
    }

    let pokemon1Pretty = prettifyPokemonProp(pokemon1);
    let pokemon2Pretty = new Object;
    let keyClassNames = new Object;
    let differenceColumn = new Object;

    if (pokemon1 && pokemon2) {
        pokemon2Pretty = prettifyPokemonProp(pokemon2);

        for (let i = 0; i < keyOrder.length; i++) {
            const key = keyOrder[i];

            if (key == "pokedex_number" || key == "generation") {
                differenceColumn[key] = "-";
                continue;
            }

            if (typeof pokemon1[key] == 'number') {
                differenceColumn[key] = (pokemon1[key] - pokemon2[key]).toFixed(1);

                if (pokemon1[key] == pokemon2[key]) {
                    keyClassNames[key] = "";
                }
                else if (pokemon1[key] > pokemon2[key]) {
                    if (damageKeys.includes(key)) { // Inverted - lower multiplier value is favourable
                        keyClassNames[key] = "disadvantage";
                    }
                    else {
                        keyClassNames[key] = "advantage";
                    }
                }
                else if (pokemon1[key] < pokemon2[key]) {
                    if (damageKeys.includes(key)) { // Inverted - lower multiplier value is favourable
                        keyClassNames[key] = "advantage";
                    }
                    else {
                        keyClassNames[key] = "disadvantage";
                    }
                }
                else {
                    keyClassNames[key] = "";
                }
            }
            else { differenceColumn[key] = "-"; }
        };
    }

    const statsTableData = keyOrder.map(key => (
        <tr key={key}>
            <td className="pokemon-property-name">{pokemonProperties_Pretty[key]}</td>
            <td className={pokemon2 ? keyClassNames[key] : ""}>
                {
                    pokemon1Pretty[key]
                }
            </td>
            {pokemon2 &&
                <>
                    <td className={keyClassNames[key]}>
                        {
                            differenceColumn[key]
                        }
                    </td>
                    <td>
                        {
                            pokemon2Pretty[key]
                        }
                    </td>
                </>
            }

        </tr>
    ));

    return (
        <table className="center">
            <thead>
                <tr>
                    <th></th>
                    <th className="table-heading">
                        {pokemon1.name}
                        <AddOrRemoveFavouritesButton pokedexNum={pokemon1.pokedex_number} />
                    </th>
                    {
                        pokemon2 &&
                        <>
                            <th></th>
                            <th className="table-heading">
                                {pokemon2.name}
                                <AddOrRemoveFavouritesButton pokedexNum={pokemon2.pokedex_number} />
                            </th>
                        </>
                    }
                </tr>
                <tr>
                    <th></th>
                    <th>
                        <img src={`data:image/${pokemon1.fileName.split('.').pop()};base64,${pokemon1.imageBase64Data}`} alt={pokemon1.fileName} />
                    </th>
                    {
                        pokemon2 &&
                        <>
                            <th></th>
                            <th>
                                <img src={`data:image/${pokemon2.fileName.split('.').pop()};base64,${pokemon2.imageBase64Data}`} alt={pokemon2.fileName} />
                            </th>
                        </>
                    }
                </tr>
            </thead>
            <tbody>
                {statsTableData}
            </tbody>
        </table>
    );
};

export default TableComponent;