import React, { useState } from 'react';
import { pokemonProperties_DataTypes, pokemonProperties_Pretty, numericFilterOptions, stringFilterOptions, boolFilterOptions } from './Data/PropertyAndFilteringLists'
import AddFilterButton from './AddFilterButton';
import RemoveFilterButton from './RemoveFilterButton';

function AdvancedSearch() {
    // TODO: codemaid cleanup?

    const [apiResponse, setApiResponse] = useState([]);
    const [userInputValues, setUserInputValues] = useState([""]);
    const [selectedProperties, setSelectedProperties] = useState([Object.keys(pokemonProperties_Pretty)[0]]);
    const [selectedFilters, setSelectedFilters] = useState([stringFilterOptions[0]]);
    const [rowCount, setRowCount] = useState(1);
    const [inputWarningMsg, setInputWarningMsg] = useState(["", ""]);

    const handleSubmit = (event) => {
        event.preventDefault();

        const fetchData = async () => {
            try {
                const arrayLength = selectedProperties.length;
                let apiQueryParams = "";
                for (let i = 0; i < arrayLength; i++) {
                    apiQueryParams += `${selectedProperties[i]}=${selectedFilters[i]}&${selectedProperties[i]}=${userInputValues[i]}&`;
                }
                apiQueryParams = apiQueryParams.slice(0, -1); // Remove the final unnecessary '&' character

                const response = await fetch(`http://localhost:55558/api/pokemon/filter?${apiQueryParams}`);
                if (response.status === 200) {
                    const data = await response.json();
                    setApiResponse(data);
                } else {
                    console.log(`Request failed with status code ${response.status}`);
                    setApiResponse([]);
                }
            } catch (error) {
                console.log(error.message);
                setApiResponse([]);
            }
        };

        fetchData();
    };

    const dropdownList_pokemonProperties = Object.keys(pokemonProperties_Pretty).map((key) => (
        <option key={key} value={key}>
            {pokemonProperties_Pretty[key]}
        </option>
    ));

    const apiResponseRender = apiResponse.map((item, index) => (
        <div key={index}>
            <a href={`/pokemon-stats/${item.name}`}>
                {item.pokedex_number}. {item.name}
            </a>
        </div>
    ))

    const getFilterOptions = (selectedProperty) => {
        const dataType = pokemonProperties_DataTypes[selectedProperty];
        switch (dataType) {
            case 'int':
            case 'double':
                return numericFilterOptions;
            case 'string':
                return stringFilterOptions;
            case 'bool':
                return boolFilterOptions;
            default:
                console.log("Error: could not resolve data type for Pokemon property.");
                return [];
        }
    }

    const getDataType = (selectedProperty) => {
        return pokemonProperties_DataTypes[selectedProperty];
    }

    const getInputType = (selectedProperty) => {
        const dataType = getDataType(selectedProperty);

        switch (dataType) {
            case 'int':
            case 'double':
                return 'number';
            default:
                return 'text';
        }
    }

    const handleOnBlur = (e, index) => {
        const value = e.target.value;
        const dataType = getDataType(selectedProperties[index]);
        if (dataType == 'int' && !Number.isInteger(Number(value))) {
            setInputWarningMsg(["Please enter a valid integer.", index]);
        } else {
            setInputWarningMsg(["", ""]);
        }
    };

    const dropdownList_filterOptions = (index) =>
        getFilterOptions(selectedProperties[index]).map((key) => (
            <option key={key} value={key}>
                {key}
            </option>
        ));

    const addFilterRow = () => {
        if (rowCount >= 5) return;

        const initializeSelectedProperty = Object.keys(pokemonProperties_Pretty)[0];
        const newSelectedProperties = [...selectedProperties, initializeSelectedProperty];
        setSelectedProperties(newSelectedProperties);

        const initializeSelectedFilter = stringFilterOptions[0];
        const newSelectedFilters = [...selectedFilters, initializeSelectedFilter];
        setSelectedFilters(newSelectedFilters);

        const newUserInputValues = [...userInputValues, ""];
        setUserInputValues(newUserInputValues);

        setRowCount((prevRowCount) => Math.min(prevRowCount + 1, 5));
    }

    const removeFilterRow = (index) => {
        if (rowCount <= 1) return;

        setSelectedProperties((prevSelectedProperties) =>
            prevSelectedProperties.filter((_, i) => i !== index)
        );

        setSelectedFilters((prevSelectedFilters) =>
            prevSelectedFilters.filter((_, i) => i !== index)
        );

        setUserInputValues((prevUserInputValues) =>
            prevUserInputValues.filter((_, i) => i !== index)
        );

        setRowCount((prevRowCount) => Math.max(prevRowCount - 1, 1));
    };

    const handleOnChangePokemonProperties = (e, index) => {
        const newSelectedProperties = [...selectedProperties];
        newSelectedProperties[index] = e.target.value;
        setSelectedProperties(newSelectedProperties);

        if (newSelectedProperties[index] == "is_legendary") {
            const newSelectedFilters = [...selectedFilters];
            newSelectedFilters[index] = "TRUE"; // Need to initialize filter state, otherwise old filter value remains in the state array
            setSelectedFilters(newSelectedFilters);
        }
    };

    const handleOnChangePokemonFilters = (e, index) => {
        const newSelectedFilters = [...selectedFilters];
        newSelectedFilters[index] = e.target.value;
        setSelectedFilters(newSelectedFilters);
    };

    const handleOnChangePokemonInput = (e, index) => {
        const newUserInputValues = [...userInputValues];
        newUserInputValues[index] = e.target.value;
        setUserInputValues(newUserInputValues);
    };

    return (
        <div className="center">
            <h1>Advanced Search</h1>

            <form onSubmit={handleSubmit}>
                {Array.from({ length: rowCount }, (_, index) => (
                    <div key={index}>

                        <select
                            className="search-form-item"
                            value={selectedProperties[index]}
                            onChange={(e) => handleOnChangePokemonProperties(e, index)}
                        >
                            {dropdownList_pokemonProperties}
                        </select>

                        <select
                            className="search-form-item"
                            value={selectedFilters[index]}
                            onChange={(e) => handleOnChangePokemonFilters(e, index)}
                        >
                            {dropdownList_filterOptions(index)}
                        </select>

                        <input
                            className="search-form-item"
                            type={getInputType(selectedProperties[index])}
                            step={
                                getDataType(selectedProperties[index]) == "double" ? "any" : undefined
                            }
                            value={userInputValues[index]}
                            onChange={(e) => handleOnChangePokemonInput(e, index)}
                            onBlur={(e) => handleOnBlur(e, index)}
                            maxLength="40"
                        ></input>

                        {inputWarningMsg[1] === index && <span className="error-msg">{inputWarningMsg[0]}</span>}

                        {
                            index == 0 ? (
                                <AddFilterButton onClick={addFilterRow} />
                            ) : (
                                <RemoveFilterButton onClick={() => { removeFilterRow(index) }} />
                            )
                        }

                    </div>
                ))}
                <input type="submit" value="Submit" />
            </form>

            <div>
                {apiResponseRender}
            </div>

            <img src="/pewter.gif" alt="Pewter City Gym Gif"></img>
        </div>
    )
}

export default AdvancedSearch;