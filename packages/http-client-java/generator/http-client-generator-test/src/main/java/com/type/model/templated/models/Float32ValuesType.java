// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) TypeSpec Code Generator.

package com.type.model.templated.models;

import com.azure.core.annotation.Generated;
import com.azure.core.annotation.Immutable;
import com.azure.json.JsonReader;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;
import java.util.List;

/**
 * An instantiated type representing float32 values type.
 */
@Immutable
public final class Float32ValuesType extends Float32Type {
    /*
     * The Kind of the Float32ValuesType.
     */
    @Generated
    private final String kind = "Float32Values";

    /**
     * Creates an instance of Float32ValuesType class.
     * 
     * @param values the values value to set.
     * @param value the value value to set.
     */
    @Generated
    public Float32ValuesType(List<Double> values, double value) {
        super(values, value);
    }

    /**
     * Get the kind property: The Kind of the Float32ValuesType.
     * 
     * @return the kind value.
     */
    @Generated
    public String getKind() {
        return this.kind;
    }

    /**
     * {@inheritDoc}
     */
    @Generated
    @Override
    public JsonWriter toJson(JsonWriter jsonWriter) throws IOException {
        jsonWriter.writeStartObject();
        jsonWriter.writeArrayField("values", getValues(), (writer, element) -> writer.writeDouble(element));
        jsonWriter.writeDoubleField("value", getValue());
        jsonWriter.writeStringField("kind", this.kind);
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of Float32ValuesType from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of Float32ValuesType if the JsonReader was pointing to an instance of it, or null if it was
     * pointing to JSON null.
     * @throws IllegalStateException If the deserialized JSON object was missing any required properties.
     * @throws IOException If an error occurs while reading the Float32ValuesType.
     */
    @Generated
    public static Float32ValuesType fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            List<Double> values = null;
            double value = 0.0;
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("values".equals(fieldName)) {
                    values = reader.readArray(reader1 -> reader1.getDouble());
                } else if ("value".equals(fieldName)) {
                    value = reader.getDouble();
                } else {
                    reader.skipChildren();
                }
            }
            return new Float32ValuesType(values, value);
        });
    }
}