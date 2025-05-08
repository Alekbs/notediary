package com.example.notediary;

public class NotUser {

    private int id;
    private String childName;
    private String childAddress;
    private String learningWorkplace;
    private String childContact;

    private String motherName;
    private String motherPassport;
    private String motherAddress;
    private String motherWorkplace;
    private String motherContact;

    private String fatherName;
    private String fatherPassport;
    private String fatherAddress;
    private String fatherWorkplace;
    private String fatherContact;

    private String email;


    // Конструктор
    public NotUser(String childName, String childAddress, String learningWorkplace, String childContact,
                           String motherName, String motherPassport, String motherAddress, String motherWorkplace, String motherContact,
                           String fatherName, String fatherPassport, String fatherAddress, String fatherWorkplace, String fatherContact,
                           String email) {
        this.childName = childName;
        this.childAddress = childAddress;
        this.learningWorkplace = learningWorkplace;
        this.childContact = childContact;

        this.motherName = motherName;
        this.motherPassport = motherPassport;
        this.motherAddress = motherAddress;
        this.motherWorkplace = motherWorkplace;
        this.motherContact = motherContact;

        this.fatherName = fatherName;
        this.fatherPassport = fatherPassport;
        this.fatherAddress = fatherAddress;
        this.fatherWorkplace = fatherWorkplace;
        this.fatherContact = fatherContact;

        this.email = email;


    }

    // Геттеры и сеттеры
    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getChildName() {
        return childName;
    }

    public void setChildName(String childName) {
        this.childName = childName;
    }

    public String getChildAddress() {
        return childAddress;
    }

    public void setChildAddress(String childAddress) {
        this.childAddress = childAddress;
    }

    public String getLearningWorkplace() {
        return learningWorkplace;
    }

    public void setLearningWorkplace(String learningWorkplace) {
        this.learningWorkplace = learningWorkplace;
    }

    public String getChildContact() {
        return childContact;
    }

    public void setChildContact(String childContact) {
        this.childContact = childContact;
    }

    public String getMotherName() {
        return motherName;
    }

    public void setMotherName(String motherName) {
        this.motherName = motherName;
    }

    public String getMotherPassport() {
        return motherPassport;
    }

    public void setMotherPassport(String motherPassport) {
        this.motherPassport = motherPassport;
    }

    public String getMotherAddress() {
        return motherAddress;
    }

    public void setMotherAddress(String motherAddress) {
        this.motherAddress = motherAddress;
    }

    public String getMotherWorkplace() {
        return motherWorkplace;
    }

    public void setMotherWorkplace(String motherWorkplace) {
        this.motherWorkplace = motherWorkplace;
    }

    public String getMotherContact() {
        return motherContact;
    }

    public void setMotherContact(String motherContact) {
        this.motherContact = motherContact;
    }

    public String getFatherName() {
        return fatherName;
    }

    public void setFatherName(String fatherName) {
        this.fatherName = fatherName;
    }

    public String getFatherPassport() {
        return fatherPassport;
    }

    public void setFatherPassport(String fatherPassport) {
        this.fatherPassport = fatherPassport;
    }

    public String getFatherAddress() {
        return fatherAddress;
    }

    public void setFatherAddress(String fatherAddress) {
        this.fatherAddress = fatherAddress;
    }

    public String getFatherWorkplace() {
        return fatherWorkplace;
    }

    public void setFatherWorkplace(String fatherWorkplace) {
        this.fatherWorkplace = fatherWorkplace;
    }

    public String getFatherContact() {
        return fatherContact;
    }

    public void setFatherContact(String fatherContact) {
        this.fatherContact = fatherContact;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}
