package org.example.creatfirtsapi.dto;

import jakarta.persistence.*;

@Entity
@Table(name = "categories")
public class CategoryDto    {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;


    @Column(name = "cat_name", nullable = false)
    private String catName;

    public CategoryDto() {

    }

    public CategoryDto( String catName) {

        this.catName = catName;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getCatName() {
        return catName;
    }

    public void setCatName(String catName) {
        this.catName = catName;
    }
}
