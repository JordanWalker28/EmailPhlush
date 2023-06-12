import styled from 'styled-components';

export const SelectContainer = styled.div`
  display: inline-flex;
  flex-direction: row;
  margin-bottom: 1rem;
`;

export const SelectLabel = styled.label`
  margin-right: 0.5rem;
  margin: auto;
  padding: 0.8rem;
`;

export const SelectDropdownElement = styled.select`
  padding: 0.5rem;
  font-size: 1rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  position: relative;
`;

export const SelectDropdownMenu = styled.select`
  position: absolute;
  top: 100%;
  left: 0;
`;
